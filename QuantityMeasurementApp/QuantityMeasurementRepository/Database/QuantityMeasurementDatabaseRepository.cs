using Microsoft.Data.SqlClient;
using QuantityMeasurementModel;
using QuantityMeasurementRepository.Exceptions;
using QuantityMeasurementRepository.Util;
using System.Data;

namespace QuantityMeasurementRepository.Database
{
    /// <summary>
    /// UC16: ADO.NET SQL Server repository — connects directly to SSMS.
    ///
    /// All SQL is executed through SqlCommand with AddWithValue() parameters —
    /// equivalent to Java JDBC PreparedStatement. SQL injection is prevented
    /// because values are always bound as parameters, never concatenated.
    ///
    /// Connection string is read from appsettings.json via DatabaseConfig.
    /// Connection pooling is handled by ConnectionPool.
    ///
    /// Does NOT touch any existing UC15 files.
    /// Implements IQuantityMeasurementRepository (UC15 + UC16 methods).
    /// </summary>
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly ConnectionPool _pool;

        // ── Constructor ────────────────────────────────────────────────────
        public QuantityMeasurementDatabaseRepository(DatabaseConfig? config = null)
        {
            config ??= new DatabaseConfig();
            _pool = new ConnectionPool(config);
            Console.WriteLine("[DatabaseRepository] Connected to SQL Server via ADO.NET.");
            Console.WriteLine($"[DatabaseRepository] {_pool.GetStatistics()}");
        }

        // ══════════════════════════════════════════════════════════════════
        // UC15 INTERFACE METHODS
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Saves one entity to SQL Server using stored procedure sp_SaveMeasurement.
        ///
        /// ADO.NET pattern used here:
        ///   new SqlCommand("sp_SaveMeasurement", connection)
        ///   cmd.CommandType = CommandType.StoredProcedure
        ///   cmd.Parameters.AddWithValue("@param", value)
        ///   cmd.ExecuteNonQuery()
        ///
        /// Equivalent to Java:
        ///   PreparedStatement pstmt = conn.prepareStatement(SQL)
        ///   pstmt.setString(1, value)
        ///   pstmt.executeUpdate()
        /// </summary>
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            SqlConnection conn = _pool.GetConnection();
            try
            {
                using SqlCommand cmd = new SqlCommand("sp_SaveMeasurement", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                // ── Bind all parameters using AddWithValue ─────────────────
                cmd.Parameters.AddWithValue("@operation_type",       entity.OperationType);
                cmd.Parameters.AddWithValue("@measurement_category", entity.MeasurementCategory);

                // Use DBNull.Value for nulls — ADO.NET requirement
                cmd.Parameters.AddWithValue("@operand1_value",
                    (object?)entity.Operand1?.Value    ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@operand1_unit",
                    (object?)entity.Operand1?.UnitName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@operand2_value",
                    (object?)entity.Operand2?.Value    ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@operand2_unit",
                    (object?)entity.Operand2?.UnitName ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@result_value",
                    (object?)entity.Result?.Value      ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@result_unit",
                    (object?)entity.Result?.UnitName   ?? DBNull.Value);

                cmd.Parameters.AddWithValue("@timestamp", entity.Timestamp);
                cmd.Parameters.AddWithValue("@has_error", entity.HasError ? 1 : 0);
                cmd.Parameters.AddWithValue("@error_message",
                    string.IsNullOrEmpty(entity.ErrorMessage)
                        ? DBNull.Value
                        : (object)entity.ErrorMessage);

                // OUTPUT parameter: SQL Server returns the new auto-increment ID
                SqlParameter newIdParam = new SqlParameter("@new_id", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(newIdParam);

                cmd.ExecuteNonQuery();

                int newId = (int)newIdParam.Value!;
                Console.WriteLine($"[DatabaseRepository] Saved ID={newId} " +
                                  $"Op={entity.OperationType} Cat={entity.MeasurementCategory}");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException($"SQL Server error saving measurement: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Failed to save measurement.", ex);
            }
            finally
            {
                _pool.ReturnConnection(conn);   // ALWAYS return to pool
            }
        }

        /// <summary>
        /// Retrieves all records ordered by timestamp.
        ///
        /// ADO.NET pattern:
        ///   SqlDataReader reader = cmd.ExecuteReader()
        ///   while (reader.Read()) { MapRow(reader) }
        ///
        /// Equivalent to Java:
        ///   ResultSet rs = pstmt.executeQuery()
        ///   while (rs.next()) { mapRow(rs) }
        /// </summary>
        public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
        {
            SqlConnection conn = _pool.GetConnection();
            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetAllMeasurements", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                return ReadEntities(cmd);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    $"SQL Server error retrieving measurements: {ex.Message}", ex);
            }
            finally
            {
                _pool.ReturnConnection(conn);
            }
        }

        /// <summary>Deletes all records. Equivalent to Java deleteAll().</summary>
        public void Clear()
        {
            SqlConnection conn = _pool.GetConnection();
            try
            {
                using SqlCommand cmd = new SqlCommand("sp_DeleteAllMeasurements", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                Console.WriteLine("[DatabaseRepository] All measurements deleted.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    $"SQL Server error deleting measurements: {ex.Message}", ex);
            }
            finally
            {
                _pool.ReturnConnection(conn);
            }
        }

        // ══════════════════════════════════════════════════════════════════
        // UC16 ADDITIONAL METHODS
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Filters by operation type using AddWithValue — SQL injection safe.
        /// Generates: WHERE operation_type = @operation_type
        /// </summary>
        public IReadOnlyList<QuantityMeasurementEntity> GetByOperationType(string operationType)
        {
            if (string.IsNullOrWhiteSpace(operationType))
                throw new ArgumentException("Operation type cannot be empty.");

            SqlConnection conn = _pool.GetConnection();
            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetByOperationType", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@operation_type", operationType.ToUpperInvariant());
                return ReadEntities(cmd);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    $"SQL Server error querying by operation '{operationType}': {ex.Message}", ex);
            }
            finally
            {
                _pool.ReturnConnection(conn);
            }
        }

        /// <summary>
        /// Filters by measurement category using AddWithValue — SQL injection safe.
        /// Generates: WHERE measurement_category = @measurement_category
        /// </summary>
        public IReadOnlyList<QuantityMeasurementEntity> GetByCategory(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty.");

            SqlConnection conn = _pool.GetConnection();
            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetByCategory", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@measurement_category", category.ToUpperInvariant());
                return ReadEntities(cmd);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    $"SQL Server error querying by category '{category}': {ex.Message}", ex);
            }
            finally
            {
                _pool.ReturnConnection(conn);
            }
        }

        /// <summary>
        /// Returns total count using ExecuteScalar() — SELECT COUNT(*).
        /// Equivalent to Java: (int) pstmt.executeQuery().getInt(1)
        /// </summary>
        public int GetTotalCount()
        {
            SqlConnection conn = _pool.GetConnection();
            try
            {
                using SqlCommand cmd = new SqlCommand("sp_GetTotalCount", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                object? result = cmd.ExecuteScalar();
                return result == null ? 0 : Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(
                    $"SQL Server error getting count: {ex.Message}", ex);
            }
            finally
            {
                _pool.ReturnConnection(conn);
            }
        }

        /// <summary>Equivalent to Java getPoolStatistics().</summary>
        public string GetRepositoryInfo()
            => $"SQL Server (ADO.NET) Repository | {_pool.GetStatistics()} | Records: {GetTotalCount()}";

        /// <summary>Closes all SQL connections. Equivalent to Java releaseResources().</summary>
        public void ReleaseResources()
        {
            _pool.Dispose();
            Console.WriteLine("[DatabaseRepository] All SQL Server connections released.");
        }

        // ══════════════════════════════════════════════════════════════════
        // PRIVATE HELPERS
        // ══════════════════════════════════════════════════════════════════

        /// <summary>
        /// Executes a stored procedure and maps every row to a domain entity.
        /// ADO.NET: cmd.ExecuteReader() + reader.Read() loop
        /// Equivalent to Java: pstmt.executeQuery() + rs.next() loop
        /// </summary>
        private static List<QuantityMeasurementEntity> ReadEntities(SqlCommand cmd)
        {
            var list = new List<QuantityMeasurementEntity>();
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
                list.Add(MapRow(reader));
            return list;
        }

        /// <summary>
        /// Maps one SqlDataReader row → QuantityMeasurementEntity.
        ///
        /// ADO.NET column reading:
        ///   reader["column_name"]          — raw object
        ///   reader["col"] == DBNull.Value  — column is NULL in database
        ///   Convert.ToDouble(reader["x"])  — typed conversion
        ///
        /// Equivalent to Java ResultSet:
        ///   rs.getString("column_name")
        ///   rs.getDouble("operand1_value")
        ///   rs.wasNull()
        /// </summary>
        private static QuantityMeasurementEntity MapRow(SqlDataReader reader)
        {
            string   operationType       = reader["operation_type"].ToString()!;
            string   measurementCategory = reader["measurement_category"].ToString()!;
            bool     hasError            = Convert.ToBoolean(reader["has_error"]);
            string   errorMsg            = reader["error_message"] == DBNull.Value
                                           ? string.Empty
                                           : reader["error_message"].ToString()!;

            QuantityDTO? operand1 = ReadDto(reader, "operand1_value", "operand1_unit", measurementCategory);
            QuantityDTO? operand2 = ReadDto(reader, "operand2_value", "operand2_unit", measurementCategory);
            QuantityDTO? result   = ReadDto(reader, "result_value",   "result_unit",   measurementCategory);

            if (hasError)
                return new QuantityMeasurementEntity(operationType, operand1, operand2, errorMsg);

            if (operand2 != null && result != null)
                return new QuantityMeasurementEntity(operationType, operand1!, operand2, result);

            return new QuantityMeasurementEntity(operationType, operand1!, result);
        }

        /// <summary>
        /// Reads two columns from the current reader row and builds a QuantityDTO.
        /// Returns null if the value column is DBNull.
        /// </summary>
        private static QuantityDTO? ReadDto(
            SqlDataReader reader,
            string        valueColumn,
            string        unitColumn,
            string        category)
        {
            if (reader[valueColumn] == DBNull.Value)
                return null;

            double value    = Convert.ToDouble(reader[valueColumn]);
            string unitName = reader[unitColumn] == DBNull.Value
                              ? string.Empty
                              : reader[unitColumn].ToString()!;

            return new QuantityDTO(value, unitName, category);
        }
    }
}
