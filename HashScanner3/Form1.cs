using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace HashScanner3
{
    public partial class HashScanner3 : Form
    {
        public HashScanner3()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            // label3'e tıklanıldığında folderBrowserDialog1'i kullanarak dizin seçme işlemi
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                // Seçilen dizini textBox3'e yazdırma
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // textBox2 içerisindeki SHA-256 hash kodunu al
            string targetHash = textBox2.Text.ToLower(); // Küçük harfe dönüştür

            try
            {
                // textBox3 içerisindeki dizindeki tüm dosya ve alt klasörlerin listesini al
                string[] allFiles = Directory.GetFiles(textBox3.Text, "*", SearchOption.AllDirectories);
                bool found = false;

                foreach (string file in allFiles)
                {
                    try
                    {
                        // Dosyanın hash değerini hesapla
                        string fileHash = CalculateFileSHA256(file);
                        Console.WriteLine("Dosya Yolu: " + file); // Dosya yolunu konsola yazdır
                        Console.WriteLine("Hash Değeri: " + fileHash); // Dosyanın hash değerini konsola yazdır

                        // Eğer dosya hash kodu hedef hash kodu ile eşleşiyorsa, dosya bulundu demektir
                        if (fileHash.Equals(targetHash))
                        {
                            // Bulunan dosyanın bilgilerini textBox1'e yaz
                            textBox1.AppendText("Dosya Bulundu:" + Environment.NewLine +
                                                 "Klasör: " + Path.GetDirectoryName(file) + Environment.NewLine +
                                                 "Dosya Adı: " + Path.GetFileName(file) + Environment.NewLine);
                            found = true;
                        }
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        textBox1.AppendText($"Erişim Hatası: {ex.Message}. Dosya: {file}" + Environment.NewLine);
                    }
                    catch (Exception ex)
                    {
                        textBox1.AppendText($"Bilinmeyen Hata: {ex.Message}. Dosya: {file}" + Environment.NewLine);
                    }
                }

                // Eğer hedef hash koduna sahip dosya bulunamazsa, kullanıcıya mesaj göster
                if (!found)
                {
                    MessageBox.Show("Belirtilen SHA-256 Hash Koduna sahip dosya bulunamadı.", "Dosya Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                MessageBox.Show($"Belirtilen dizin bulunamadı. Hata: {ex.Message}", "Dizin Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"Erişim reddedildi. Hata: {ex.Message}", "Erişim Reddedildi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bilinmeyen bir hata oluştu. Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Dosyanın SHA-256 hash değerini hesaplayan fonksiyon
        private string CalculateFileSHA256(string filePath)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = sha256.ComputeHash(fileStream);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
        }
    }
}
