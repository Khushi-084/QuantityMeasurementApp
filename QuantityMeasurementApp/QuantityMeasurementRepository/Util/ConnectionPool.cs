using Microsoft.Data.SqlClient;

namespace QuantityMeasurementRepository.Util
{
    /// <summary>
    /// UC16: Manages a pool of reusable SqlConnection objects for SQL Server.
    /// Equivalent to Java ConnectionPool.java.
    ///
    /// Key ADO.NET methods:
    ///   new SqlConnection(connectionString) — creates connection object
    ///   connection.Open()                   — opens physical connection to SQL Server
    ///   connection.State                    — checks if connection is open/closed
    /// </summary>
    public class ConnectionPool : IDisposable
    {
        private readonly Queue<SqlConnection> _available = new();
        private readonly List<SqlConnection>  _all       = new();
        private readonly object               _lock      = new();
        private readonly string               _connectionString;
        private readonly int                  _maxSize;
        private          int                  _activeCount;
        private          bool                 _disposed;

        public ConnectionPool(DatabaseConfig config)
        {
            _connectionString = config.ConnectionString;
            _maxSize          = config.MaxPoolSize;

            // Pre-warm: open all connections upfront
            for (int i = 0; i < _maxSize; i++)
            {
                var conn = new SqlConnection(_connectionString);
                conn.Open();
                _available.Enqueue(conn);
                _all.Add(conn);
            }

            Console.WriteLine($"[ConnectionPool] Initialized {_maxSize} SQL Server connections.");
        }

        /// <summary>
        /// Borrow a connection. Equivalent to Java ConnectionPool.getConnection().
        /// </summary>
        public SqlConnection GetConnection()
        {
            lock (_lock)
            {
                int waited = 0;
                while (_available.Count == 0)
                {
                    System.Threading.Monitor.Wait(_lock, 1000);
                    waited++;
                    if (waited >= 30)
                        throw new Exceptions.DatabaseException(
                            "Connection pool exhausted — no connections available after 30s.");
                }

                var conn = _available.Dequeue();
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();

                _activeCount++;
                return conn;
            }
        }

        /// <summary>
        /// Return a connection to the pool. ALWAYS call in a finally block.
        /// Equivalent to Java ConnectionPool.release().
        /// </summary>
        public void ReturnConnection(SqlConnection connection)
        {
            lock (_lock)
            {
                _available.Enqueue(connection);
                _activeCount--;
                System.Threading.Monitor.Pulse(_lock);
            }
        }

        /// <summary>Equivalent to Java getPoolStatistics().</summary>
        public string GetStatistics()
        {
            lock (_lock)
            {
                return $"Total: {_maxSize} | Active: {_activeCount} | Available: {_available.Count}";
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            foreach (var conn in _all)
            {
                try { conn.Close(); conn.Dispose(); }
                catch { /* best effort */ }
            }
            Console.WriteLine("[ConnectionPool] All connections closed.");
        }
    }
}
