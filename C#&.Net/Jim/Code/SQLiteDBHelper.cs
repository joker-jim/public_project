
using Jim.Code;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace Jim
{
    public class SQLiteDBHelper
    {
        private string _connectionString;

        public SQLiteDBHelper(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;FailIfMissing=False;";
        }

        public async Task ConnectAsync()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    Console.WriteLine("Connected to the SQLite database successfully.");
                   
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("An error occurred while connecting to the SQLite database: " + ex.Message);
                }
            }
        }

        public async Task ExecuteQueryAsync(string query)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand(query, connection))
                {
                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine("Query executed successfully.");
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("An error occurred while executing the query: " + ex.Message);
                    }
                }
            }
        }

        public async Task<bool> UserLogin(string username, string enteredPassword)
        {
            
            string hashedPassword;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                hashedPassword = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT COUNT(1) FROM Customer WHERE name = @username AND password = @password", connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", hashedPassword);

                    var result = await command.ExecuteScalarAsync();
                    return result != null && Convert.ToInt32(result) > 0;
                }
            }
        }

        public async Task<UserInfo> GetUserInfo(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT name, address, contact,email,phone FROM Customer WHERE name = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new UserInfo
                            {
                                name = reader["name"].ToString(),
                                address = reader["address"].ToString(),
                                contact = reader["contact"].ToString(),
                                email = reader["email"].ToString(),
                                phone = reader["phone"].ToString(),
                            };
                        }
                    }
                }
            }
            return null; 
        }
        
        public async Task<bool> AddToFaultAsync(string code, string hist, string name)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

            
                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM product WHERE code = @code AND name = @name", connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    command.Parameters.AddWithValue("@name", name);
                    long count = (long)await command.ExecuteScalarAsync();

                    if (count == 0)
                    {
                        return false;
                    }
                    
                }


                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Fault WHERE code = @code", connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    long count = (long)await command.ExecuteScalarAsync();

                    if (count > 0)
                    {
                        return false;
                    }
                }

                using (var command = new SQLiteCommand("INSERT INTO Fault (code, hist, customer, record_date) VALUES (@code, @hist, @customer, DATE('now'))", connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    command.Parameters.AddWithValue("@hist", hist);
                    command.Parameters.AddWithValue("@customer", UserCache.CurrentUser.name);
                    await command.ExecuteNonQueryAsync();
                }

                return true;
            }
        }
        public async Task<DataTable> GetFaultDataByUserAsync(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Fault WHERE customer = @username";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@username", username);
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

        public async Task UpdateDatabaseAsync(DataTable table)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        foreach (DataRow row in table.Rows)
                        {
                            if (row.RowState == DataRowState.Modified)
                            {
                              
                                using (var checkCommand = new SQLiteCommand("SELECT status FROM Fault WHERE code = @code", connection))
                                {
                                    checkCommand.Parameters.AddWithValue("@code", row["code"]);
                                    var status = await checkCommand.ExecuteScalarAsync();
                                    if (status != null && status != DBNull.Value)
                                    {
                                        MessageBox.Show($"Cannot be modified because of order {row["code"]}. It's done");
                                        continue; 
                                    }
                                }
 
                                using (var updateCommand = new SQLiteCommand("UPDATE Fault SET hist = @hist WHERE code = @code", connection))
                                {
                                    updateCommand.Transaction = transaction;
                                    updateCommand.Parameters.AddWithValue("@hist", row["hist"]);
                                    updateCommand.Parameters.AddWithValue("@code", row["code"]);
                                    await updateCommand.ExecuteNonQueryAsync();
                                }
                            }
                        }
                        transaction.Commit();
                    }
                    catch (SQLiteException ex)
                    {
                        MessageBox.Show( ex.Message);
                        transaction.Rollback();
                    }
                }
            }
        }




        public async Task<bool> DeleteFaultRowAsync(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("DELETE FROM Fault WHERE code = @code", connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    int affectedRows = await command.ExecuteNonQueryAsync();
                    return affectedRows > 0;
                }
            }
        }

        /***********************************************************************************************************************************************************/


        //employee
        public async Task<(bool, string)> EmployeeLogin(string username, string rawPassword)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                using (var command = new SQLiteCommand("SELECT password, type FROM Employee WHERE name = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string storedHash = reader["password"].ToString();
                            string userType = reader["type"].ToString();

                         
                            bool isVerified = BCrypt.Net.BCrypt.Verify(rawPassword, storedHash);

                            if (isVerified)
                            {
                                return (true, userType); 
                            }
                        }
                    }
                }
            }
            return (false, null); 
        }

        public async Task<EmployeeInfo> GetEmployInfo(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT name, address, type FROM Employee WHERE name = @username", connection))
                {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new EmployeeInfo
                            {
                                name = reader["name"].ToString(),
                                address = reader["address"].ToString(),
                                type = reader["type"].ToString()
                            };
                        }
                    }
                }
            }
            return null; 
        }

   

        public async Task<DataTable> SearchCustomerByNameOrContactAsync(string searchTerm)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT name, address, contact,password, email, phone FROM Customer WHERE name LIKE @searchTerm OR contact LIKE @searchTerm";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }
        public async Task<bool> AddCustomerAsync(string name, string address, string contact, string password, string email, string phone)
        {
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                throw new ArgumentException("Email is not in a valid format.");
            }

            
            if (!Regex.IsMatch(phone, @"^\d{1,10}$"))
            {
                throw new ArgumentException("Phone number must be numeric and 10 digits or less.");
            }

            
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

               
                using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Customer WHERE name = @name OR contact = @contact", connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@contact", contact);
                    long count = (long)await cmd.ExecuteScalarAsync();
                    if (count > 0)
                    {
                        return false;
                    }
                }

                
                using (var cmd = new SQLiteCommand("INSERT INTO Customer (name, address, contact, password, email, phone) VALUES (@name, @address, @contact, @password, @email, @phone)", connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@contact", contact);
                    cmd.Parameters.AddWithValue("@password", password); // Now using hashed password
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            return true;
        }

        public async Task<bool> UpdateCustomerAsync(string originalName, string newName, string address, string contact, string email, string phone)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (!originalName.Equals(newName))
                        {
                            using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Fault WHERE customer = @customer", connection, transaction))
                            {
                                cmd.Parameters.AddWithValue("@customer", originalName);
                                long count = (long)await cmd.ExecuteScalarAsync();
                                if (count > 0)
                                {
                                    MessageBox.Show("Cannot update the name because it has associated records in the Fault table.");
                                    return false;
                                }
                            }
                        }

                        using (var cmd = new SQLiteCommand("UPDATE Customer SET name = @newName, address = @address, contact = @contact, email = @email, phone = @phone WHERE name = @originalName", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@originalName", originalName);
                            cmd.Parameters.AddWithValue("@newName", newName);
                            cmd.Parameters.AddWithValue("@address", address);
                            cmd.Parameters.AddWithValue("@contact", contact);
                            cmd.Parameters.AddWithValue("@email", email);
                            cmd.Parameters.AddWithValue("@phone", phone);

                            int affectedRows = await cmd.ExecuteNonQueryAsync();
                            if (affectedRows == 0)
                            {
                                MessageBox.Show("No rows were updated.");
                                return false;
                            }
                        }

                        transaction.Commit();
                        MessageBox.Show("Customer(s) updated successfully!");
                        return true;
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show("An error occurred: " + ex.Message);
                        return false;
                    }
                }
            }
        }
        public async Task<string> DeleteCustomerAsync(string username)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Fault WHERE customer = @customer", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@customer", username);
                            long count = (long)await cmd.ExecuteScalarAsync();
                            if (count > 0)
                            {
                                return "Cannot delete the customer because there are associated records in the Fault table.";
                            }
                        }

                        using (var cmd = new SQLiteCommand("DELETE FROM Customer WHERE name = @username", connection, transaction))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            int affectedRows = await cmd.ExecuteNonQueryAsync();
                            if (affectedRows == 0)
                            {
                                return "No customer found with the specified username.";
                            }
                        }

                        transaction.Commit();
                        return "Customer deleted successfully!";
                    }
                    catch (SQLiteException ex)
                    {
                        transaction.Rollback();
                        return "An error occurred: " + ex.Message;
                    }
                }
            }
        }
        public async Task<DataTable> SearchFaultsByCustomerOrCodeAsync(string searchTerm)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Fault WHERE customer LIKE @searchTerm OR code LIKE @searchTerm";
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                    using (var adapter = new SQLiteDataAdapter(command))
                    {
                        DataTable table = new DataTable();
                        adapter.Fill(table);
                        return table;
                    }
                }
            }
        }

     

        public async Task<bool> CustomerExistsAsync(string name, string contact)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Customer WHERE name = @name AND contact = @contact", connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@contact", contact);
                    long count = (long)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> ProductExistsAsync(string productName, string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Product WHERE name = @productName AND code = @code", connection))
                {
                    command.Parameters.AddWithValue("@productName", productName);
                    command.Parameters.AddWithValue("@code", code);
                    long count = (long)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        public async Task<bool> CodeIsUniqueAsync(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand("SELECT COUNT(*) FROM Fault WHERE code = @code", connection))
                {
                    command.Parameters.AddWithValue("@code", code);
                    long count = (long)await command.ExecuteScalarAsync();
                    return count == 0;
                }
            }
        }

        public async Task<bool> AddFaultRecordAsync(string name, string code, string hist, DateTime recordDate)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand("INSERT INTO Fault (customer, code, hist, record_date) VALUES (@name, @code, @hist, @recordDate)", connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@code", code);
                    command.Parameters.AddWithValue("@hist", hist);
                    command.Parameters.AddWithValue("@recordDate", recordDate.ToString("yyyy-MM-dd"));
                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

     
        public async Task<bool> CanUpdateHist(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT COUNT(1) FROM Fault WHERE code = @code AND (status IS NOT NULL OR developer IS NOT NULL OR type IS NOT NULL OR pri IS NOT NULL)", connection))
                {
                    command.Parameters.AddWithValue("@code", code);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) == 0; 
                }
            }
        }

        public async Task UpdateHist(string code, string newHistValue)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("UPDATE Fault SET hist = @newHistValue WHERE code = @code", connection))
                {
                    command.Parameters.AddWithValue("@newHistValue", newHistValue);
                    command.Parameters.AddWithValue("@code", code);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<bool> CanDeleteRow(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT COUNT(1) FROM Fault WHERE code = @code AND (status IS NOT NULL OR developer IS NOT NULL OR type IS NOT NULL OR pri IS NOT NULL)", connection))
                {
                    command.Parameters.AddWithValue("@code", code);

                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) == 0;
                }
            }
        }

        public async Task DeleteRow(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("DELETE FROM Fault WHERE code = @code", connection))
                {
                    command.Parameters.AddWithValue("@code", code);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        //Manager

        public async Task<DataTable> GetDataAsync(string query, params SQLiteParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    if (parameters != null && parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        var dt = new DataTable();
                        await connection.OpenAsync();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }
        //manager order
        public async Task<bool> EmployeeExistsAsync(string name, string type)
        {
            string query = "SELECT COUNT(*) FROM Employee WHERE name = @name AND type = @type";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@type", type);
                    await connection.OpenAsync();
                    var result = await cmd.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 0;
                }
            }
        }

        public async Task UpdateFaultAsync(string code, string developer, string pri, string type)
        {
            string query = "UPDATE Fault SET developer = @developer, pri = @pri, type = @type WHERE code = @code";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@developer", developer);
                    cmd.Parameters.AddWithValue("@pri", pri);
                    cmd.Parameters.AddWithValue("@type", type);
                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task InsertInformationAsync(string managerName, string developerName, string code, string type)
        {
            string query = "INSERT INTO Information (manager_name, developer_name, code, type) VALUES (@managerName, @developerName, @code, @type)";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@managerName", managerName);
                    cmd.Parameters.AddWithValue("@developerName", developerName);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@type", type);
                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteInformationAsync(string code, string developer)
        {
            string query = "DELETE FROM Information WHERE code = @code AND developer_name = @developer";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@developer", developer);
                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task ClearFaultFieldsAsync(string code)
        {
            string query = "UPDATE Fault SET pri = NULL, developer = NULL, type = NULL WHERE code = @code";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@code", code);
                    await connection.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        //manager talk

        public async Task<List<InformationRecord>> GetInformationByManagerName(string managerName)
        {
            var resultList = new List<InformationRecord>();
            string query = "SELECT code, developer_name, type1 FROM Information WHERE manager_name = @managerName";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@managerName", managerName);
                    await connection.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var record = new InformationRecord
                            {
                                Code = reader.GetValue(0).ToString(),
                                DeveloperName = reader.GetValue(1).ToString(),
                                Type1 = reader.IsDBNull(2) ? null : (int?)Convert.ToInt32(reader.GetValue(2))
                            };
                            resultList.Add(record);
                        }
                    }
                }
            }
            return resultList;
        }



        public async Task<bool> UpdateInformationTypeAsync(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var cmd = new SQLiteCommand("UPDATE Information SET type1 = 0 WHERE code = @code", connection))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        var result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("An error occurred while updating the SQLite database: " + ex.Message);
                    return false;
                }
            }
        }

        public async Task<(string developer_inf, string code)> GetInformationAsync(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var cmd = new SQLiteCommand("SELECT developer_inf, code FROM Information WHERE code = @code", connection))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return (reader["developer_inf"].ToString(), reader["code"].ToString());
                            }
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("An error occurred while querying the SQLite database: " + ex.Message);
                }
                return (null, null);
            }
        }

        public async Task<bool> UpdateInformationAsync(string code, string manager_inf)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    using (var cmd = new SQLiteCommand("UPDATE Information SET type = 1, manager_inf = @manager_inf WHERE code = @code", connection))
                    {
                        cmd.Parameters.AddWithValue("@code", code);
                        cmd.Parameters.AddWithValue("@manager_inf", manager_inf);
                        var result = await cmd.ExecuteNonQueryAsync();
                        return result > 0;
                    }
                }
                catch (SQLiteException ex)
                {
                    Console.WriteLine("An error occurred while updating the SQLite database: " + ex.Message);
                    return false;
                }
            }
        }
        //developer

        public async Task<DataTable> GetDeveloperFaultsAsync(string developerName)
        {
            DataTable dt = new DataTable();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = $"SELECT * FROM Fault WHERE developer = @developerName ORDER BY CASE pri WHEN 'major' THEN 1 ELSE 2 END";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@developerName", developerName);

                    try
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            dt.Load(reader);
                        }
                        Console.WriteLine("Data retrieved successfully.");
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("An error occurred while retrieving the data: " + ex.Message);
                    }
                }
            }

            return dt;
        }

        public async Task<(string, string)> GetFaultDetailsAsync(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT type, hist FROM Fault WHERE code = @code";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@code", code);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string type = reader.GetString(0);
                            string hist = reader.GetString(1);
                            return (type, hist);
                        }
                    }
                }
            }

            return (null, null);
        }
        public async Task UpdateFaultAsync(string code, string processingReport)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Fault SET Processing_report = @processingReport, " +
                               "processing_date = @processingDate, status = @status " +
                               "WHERE code = @code";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@processingReport", processingReport);
                    command.Parameters.AddWithValue("@processingDate", DateTime.Now.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@status", "resolved");
                    command.Parameters.AddWithValue("@code", code);

                    try
                    {
                        await command.ExecuteNonQueryAsync();
                        Console.WriteLine("Fault table updated successfully.");
                    }
                    catch (SQLiteException ex)
                    {
                        Console.WriteLine("An error occurred while updating the Fault table: " + ex.Message);
                    }
                }
            }
        }

        public async Task<Information> RetrieveInformationAsync(string code)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT code, manager_inf FROM Information WHERE code = @code";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@code", code);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Information
                            {
                                Code = reader.GetString(0),
                                ManagerInf = reader.GetString(1)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public async Task<bool> UpdateInformationAsync1(string code, string developerInf)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"
            UPDATE Information
            SET developer_inf = @developerInf, type1 = 1, type = 1
            WHERE code = @code";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@developerInf", developerInf);
                    command.Parameters.AddWithValue("@code", code);

                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        //-------------------------------------------------------------------------------------//

        public async Task<DataTable> GetCustomersAsync()
        {
            DataTable dataTable = new DataTable();

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "SELECT * FROM Customer"; 

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        dataTable.Load(reader);
                    }
                }
            }

            return dataTable;
        }

        public async Task<DataTable> GetEmployeesAsync()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT * FROM Employee", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
        }
        public async Task<DataTable> GetProductsAsync()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT * FROM Product", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
        }
        public async Task<DataTable> GetFaultsAsync()
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand("SELECT * FROM Fault", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        DataTable table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
        }
        public async Task<bool> InsertCustomerAsync(string name, string address, string contact, string rawPassword, string email, string phone)
        {
           
            string hashedPassword = HashPassword(rawPassword);

          
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Email is not in a valid format.");
            }

            
            if (!IsValidPhoneNumber(phone))
            {
                throw new ArgumentException("Phone number must have 10 digits or less.");
            }

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"INSERT INTO Customer (name, address, contact, password, email, phone) 
                VALUES (@name, @address, @contact, @password, @email, @phone)";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@address", address);
                    command.Parameters.AddWithValue("@contact", contact);
                    command.Parameters.AddWithValue("@password", hashedPassword);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@phone", phone);

                    await command.ExecuteNonQueryAsync();
                    return true;
                }
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPhoneNumber(string phone)
        {
            return phone.All(Char.IsDigit) && phone.Length <= 10;
        }


        public async Task UpdateCustomerAsync(int id, string name, string address, string contact, string email, string phone)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE Customer SET 
                        name = @name, 
                        address = @address, 
                        contact = @contact, 
                        email = @email, 
                        phone = @phone
                        WHERE id = @id";

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@address", address);
                    command.Parameters.AddWithValue("@contact", contact);
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@phone", phone);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            string query = "DELETE FROM Customer WHERE id = @id";
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    int result = await command.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> AddEmployeeAsync(string name, string address, string type, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be empty.", nameof(name));
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Type cannot be empty.", nameof(type));
            }

            if (string.IsNullOrWhiteSpace(rawPassword))
            {
                throw new ArgumentException("Password cannot be empty.", nameof(rawPassword));
            }

            string hashedPassword = HashPassword1(rawPassword);

            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Employee WHERE name = @name", connection))
                {
                    checkCmd.Parameters.AddWithValue("@name", name);
                    var exists = (long)await checkCmd.ExecuteScalarAsync();
                    if (exists > 0)
                    {
                      
                        throw new ArgumentException("An employee with the same name already exists.", nameof(name));
                    }
                }

               
                string query = "INSERT INTO Employee (name, address, type, password) VALUES (@name, @address, @type, @password)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@address", address ?? string.Empty);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }


        private string HashPassword1(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }


        public async Task<bool> UpdateEmployeeAsync(int id, string name, string address, string type, string password)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Employee SET name = @name, address = @address, type = @type, password = @password WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@address", address);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@password", password);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Employee WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> AddFaultAsync(string customer, string developer, string code, DateTime recordDate, string type, string hist, string pri, DateTime processingDate, string processingReport, string status)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string checkQuery = "SELECT COUNT(1) FROM Fault WHERE code = @code AND record_date = @record_date";
                using (var checkCmd = new SQLiteCommand(checkQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@code", code);
                    checkCmd.Parameters.AddWithValue("@record_date", recordDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    int exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync());
                    if (exists > 0)
                    {
                       
                        return false;
                    }
                }

                
                string insertQuery = @"INSERT INTO Fault(customer, developer, code, record_date, type, hist, pri, processing_date, Processing_report, status) 
                 VALUES (@customer, @developer, @code, @record_date, @type, @hist, @pri, @processing_date, @Processing_report, @status)";
                using (var cmd = new SQLiteCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@customer", customer);
                    cmd.Parameters.AddWithValue("@developer", developer);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@record_date", recordDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@hist", hist);
                    cmd.Parameters.AddWithValue("@pri", pri);
                    cmd.Parameters.AddWithValue("@processing_date", processingDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Processing_report", processingReport);
                    cmd.Parameters.AddWithValue("@status", status);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }

        public async Task<bool> UpdateFaultAsync(int id, string customer, string developer, string code, DateTime recordDate, string type, string hist, string pri, DateTime processingDate, string processingReport, string status)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = @"UPDATE Fault
                         SET customer = @customer,
                             developer = @developer,
                             code = @code,
                             record_date = @record_date,
                             type = @type,
                             hist = @hist,
                             pri = @pri,
                             processing_date = @processing_date,
                             Processing_report = @Processing_report,
                             status = @status
                         WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@customer", customer);
                    cmd.Parameters.AddWithValue("@developer", developer);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@record_date", recordDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@hist", hist);
                    cmd.Parameters.AddWithValue("@pri", pri);
                    cmd.Parameters.AddWithValue("@processing_date", processingDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@Processing_report", processingReport);
                    cmd.Parameters.AddWithValue("@status", status);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> DeleteFaultAsync(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Fault WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> AddProductAsync(string name, string code, DateTime dateReleased, string release)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                
                using (var checkCmd = new SQLiteCommand("SELECT COUNT(*) FROM Product WHERE code = @code", connection))
                {
                    checkCmd.Parameters.AddWithValue("@code", code);
                    long count = (long)await checkCmd.ExecuteScalarAsync();
                    if (count > 0)
                    {
                      
                        return false;
                    }
                }

                
                string query = "INSERT INTO Product(name, code, dateReleased, release) VALUES (@name, @code, @dateReleased, @release)";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@dateReleased", dateReleased.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@release", release);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> UpdateProductAsync(int id, string name, string code, DateTime dateReleased, string release)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "UPDATE Product SET name = @name, code = @code, dateReleased = @dateReleased, release = @release WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@code", code);
                    cmd.Parameters.AddWithValue("@dateReleased", dateReleased.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@release", release);

                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            using (var connection = new SQLiteConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "DELETE FROM Product WHERE id = @id";
                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }




    }
}
