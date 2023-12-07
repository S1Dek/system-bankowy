using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace system_bankowy
{
    public class Loging
    {
        private string filePath;

        public Loging(string fileName)
        {
            string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            filePath = Path.Combine(folderPath, fileName);
        }
        public bool VerifyCredentials(string enteredLogin, string enteredPassword)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string storedLogin = parts[0].Trim();
                        string storedPassword = parts[1].Trim();

                        if (storedLogin == enteredLogin && storedPassword == enteredPassword)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                return false;
            }
        }
    }
}
