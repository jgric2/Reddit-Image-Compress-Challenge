using RedditImageCompressChallange.Compression;

namespace RedditImageCompressChallange
{
    public partial class Form1 : Form
    {
        private ImageCompressor currentGrid;
        private Bitmap originalBitmap;
        private Bitmap reconstructedBitmap;


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "PNG Images|*.png";
            openFileDialog1.Title = "Select an 8x8 PNG Image";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Bitmap bmp = new Bitmap(openFileDialog1.FileName);

                    if (bmp.Width != 8 || bmp.Height != 8)
                    {
                        MessageBox.Show("Please select an 8x8 pixel PNG image.", "Invalid Image Size", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Create PixelGrid8x8 from Bitmap
                    currentGrid = ImageCompressor.FromBitmap(bmp);
                    originalBitmap = bmp;
                    pictureBoxOriginal.Image = originalBitmap;
                    pictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;

                    MessageBox.Show("PNG loaded and processed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading PNG: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (currentGrid == null)
            {
                MessageBox.Show("Please load a PNG image first.", "No Grid Data", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            saveFileDialog1.Filter = "Binary Files|*.dat";
            saveFileDialog1.Title = "Save Serialized Grid Data";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    currentGrid.SaveToFile(saveFileDialog1.FileName);
                    MessageBox.Show("Grid data serialized and saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving serialized data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog2.Filter = "Binary Files|*.dat";
            openFileDialog2.Title = "Load Serialized Grid Data";

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Load and deserialize the grid
                    ImageCompressor loadedGrid = ImageCompressor.LoadFromFile(openFileDialog2.FileName);
                    reconstructedBitmap = loadedGrid.ToBitmap();
                    pictureBoxReconstructed.Image = reconstructedBitmap;
                    pictureBoxReconstructed.SizeMode = PictureBoxSizeMode.Zoom;

                    MessageBox.Show("Grid data deserialized and image reconstructed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deserializing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
