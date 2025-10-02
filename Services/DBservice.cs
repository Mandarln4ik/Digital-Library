using Digital_Library.Model;
using MySql.Data.MySqlClient;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Digital_Library.Services
{
    public class DBservice
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public MySqlConnection GetConnection()
        {
            if (_connection == null)
            {
                MySqlConnection connection = new MySqlConnection(_connectionString);
                _connection = connection;
                _connection.Open();
            }
            return _connection;
        }

        public DBservice(string server = "148.253.208.189", string database = "DigitalLibrary", string userId = "DLadmin", string password = "pr.e6Spcu8@cQFoy;")
        {
            _connectionString = $"Server={server};Port=3306;Database={database};Uid={userId};Pwd={password};";
        }

        public User GetUserByEmailOrNickname(string login)
        {
            User user;
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"SELECT * FROM `users` WHERE `username` = \"{login}\" OR `email` = \"{login}\";", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        user = new User()
                        {
                            Id = reader.GetInt32("id"),
                            Username = reader.GetString("username"),
                            Email = reader.GetString("email"),
                            FirstName = reader.GetString("first_name"),
                            LastName = reader.GetString("last_name"),
                            PasswordHash = reader.GetString("password_hash"),
                            isAdmin = reader.GetBoolean("isAdmin"),
                            RegistrationDate = reader.GetDateTime("registration_date"),
                            LastLogin = reader.GetDateTime("last_login")

                        };
                        return user;
                    }
                }
            }
            return null;
        }

        public void AddNewUser(string username, string email, string passwordHash, string firstname = " ", string lastname = " ")
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"INSERT INTO `users` (`id`, `username`, `email`, `password_hash`, `first_name`, `last_name`, `isAdmin`, `registration_date`, `last_login`) VALUES (NULL, '{username}', '{email}', '{passwordHash}', '', '', '0', current_timestamp(), current_timestamp());", connection);
                Debug.WriteLine(command.CommandText);
                command.ExecuteNonQuery();
            }
        }

        public List<Document> GetAllDocuments()
        {
            List<Document> docs = new();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand("SELECT * FROM `documents`", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        docs.Add(new Document()
                        {
                            DocumentId = reader.GetInt32("document_id"),
                            Title = reader.GetString("title"),
                            AlternativeTitle = reader.IsDBNull(reader.GetOrdinal("alternative_title")) ? null : reader.GetString("alternative_title"),
                            PublicationYear = reader.IsDBNull(reader.GetOrdinal("publication_year")) ? (int?)null : reader.GetInt32("publication_year"),
                            ISBN = reader.IsDBNull(reader.GetOrdinal("isbn")) ? null : reader.GetString("isbn"),
                            ISSN = reader.IsDBNull(reader.GetOrdinal("issn")) ? null : reader.GetString("issn"),
                            UDC = reader.IsDBNull(reader.GetOrdinal("udc")) ? null : reader.GetString("udc"),
                            BBK = reader.IsDBNull(reader.GetOrdinal("bbk")) ? null : reader.GetString("bbk"),
                            Description = reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString("description"),
                            TotalPages = reader.IsDBNull(reader.GetOrdinal("total_pages")) ? (int?)null : reader.GetInt32("total_pages"),
                            FilePath = reader.IsDBNull(reader.GetOrdinal("file_path")) ? null : reader.GetString("file_path"),
                            ImagePath = reader.IsDBNull(reader.GetOrdinal("image_path")) ? null : reader.GetString("image_path"),
                            DateUploaded = reader.GetDateTime("date_uploaded"),
                            UploadedBy = reader.IsDBNull(reader.GetOrdinal("uploaded_by")) ? (int?)null : reader.GetInt32("uploaded_by"),
                            LanguageId = reader.GetInt32("language_id")
                        });
                    }
                }
            }

            return docs;
        }

        public Author GetAuthorByDocumentId(int docId)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"SELECT * FROM `authors` WHERE `author_id` = (SELECT `author_id` FROM `document_authors` WHERE `document_id` = {docId});", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Author author = new Author()
                        {
                            AuthorId = reader.GetInt32("author_id"),
                            FirstName = reader.GetString("first_name"),
                            LastName = reader.GetString("last_name"),
                            MiddleName = reader.IsDBNull(reader.GetOrdinal("middle_name")) ? null : reader.GetString("middle_name")
                        };
                        return author;
                    }
                }
            }
            return null;
        }

        public Language GetLanguageById(int id)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                MySqlCommand command = new MySqlCommand($"SELECT * FROM `languages` WHERE `language_id` = {id}", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Language language = new Language()
                        {
                            LanguageId = reader.GetInt32(0),
                            LanguageCode = reader.GetString(1),
                            LanguageName = reader.GetString(2),
                        };
                        return language;
                    }
                }
            }
            return null;
        }

        public List<DocumentTag> GetAllTagsToDocument(int docId)
        {
            List<DocumentTag> tags = new();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"SELECT * FROM `document_tags` WHERE `document_id` = {docId}", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(new DocumentTag()
                        {
                            DocumentId = reader.GetInt32(0),
                            TagId = reader.GetInt32(1)
                        });
                    }
                }
            }
            return tags;
        }

        public List<Tag> GetAllTags()
        {
            List<Tag> tags = new();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"SELECT * FROM tags ORDER BY `tags`.`tag_id` ASC", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(new Tag()
                        {
                            TagId = reader.GetInt32(0),
                            TagName = reader.GetString(1)
                        });
                    }
                }
            }
            return tags;
        }

        public async Task<bool> EditUser(User newUser)
        {
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand($"UPDATE users SET email = '{newUser.Email}',\nlast_name = '{newUser.LastName}',\n first_name = '{newUser.FirstName}', \nusername = '{newUser.Username}' \nWHERE id = {newUser.Id};", connection);
                Debug.WriteLine(command.CommandText);
                int affectedRow = await command.ExecuteNonQueryAsync();
                if ( affectedRow > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


        public async Task<DataTable> GetResultsFromSQL(string query)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        await adapter.FillAsync(dataTable); // Загружаем данные в DataTable

                    }
                }
            }
            return dataTable;
        }
    }
}
