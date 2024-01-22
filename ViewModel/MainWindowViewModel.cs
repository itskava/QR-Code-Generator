using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using QR_Code_Generator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace QR_Code_Generator.ViewModel
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Settings properties

        private string _background = "#FFFFFF";

        public string Background
        {
            get { return _background; }
            set
            {
                if (VerifyColor(value))
                {
                    if (value.Length == 1 && value[0] != '#')
                    {
                        value = "#" + value;
                    }

                    _background = value.ToUpper();
                    BackgroundShowcase = new SolidColorBrush(ConvertStringToColor(_background));
                }

                OnPropertyChanged();
            }
        }


        private bool _isBackgroundEnabled = true;

        public bool IsBackgroundEnabled
        {
            get { return _isBackgroundEnabled; }
            set
            {
                _isBackgroundEnabled = value;
                OnPropertyChanged();
            }
        }

        private SolidColorBrush _backgroundShowcase = Brushes.White;

        public SolidColorBrush BackgroundShowcase
        {
            get { return _backgroundShowcase; }
            set
            {
                _backgroundShowcase = value;
                OnPropertyChanged();
            }
        }

        private string _foreground = "#000000";

        public string Foreground
        {
            get { return _foreground; }
            set
            {
                if (VerifyColor(value))
                {
                    if (value.Length == 1 && value[0] != '#')
                    {
                        value = "#" + value;
                    }

                    _foreground = value.ToUpper();
                    ForegroundShowcase = new SolidColorBrush(ConvertStringToColor(_foreground));

                }

                OnPropertyChanged();
            }
        }

        private SolidColorBrush _foregroundShowcase = Brushes.Black;

        public SolidColorBrush ForegroundShowcase
        {
            get { return _foregroundShowcase; }
            set
            {
                _foregroundShowcase = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// This method is used to convert a color (System.Windows.Media.Color) from its string hex representation.
        /// </summary>
        private static Color ConvertStringToColor(string color)
        {
            byte r = 0, g = 0, b = 0;

            if (color.Length == 7)
            {
                r = Convert.ToByte(color[1..3], 16);
                g = Convert.ToByte(color[3..5], 16);
                b = Convert.ToByte(color[5..7], 16);
            }
            else if (color.Length == 4)
            {
                r = Convert.ToByte(color[1..2] + color[1..2], 16);
                g = Convert.ToByte(color[2..3] + color[2..3], 16);
                b = Convert.ToByte(color[3..4] + color[3..4], 16);
            }

            return Color.FromRgb(r, g, b);
        }

        /// <summary>
        /// This method is used to check the correctness of the color's code.
        /// </summary>
        private static bool VerifyColor(string color)
        {
            if (color.Length > 7) return false;

            if (color.Length == 1)
            {
                char symbol = char.ToUpper(color[0]);
                if (('0' <= symbol && symbol <= '9') || ('A' <= symbol && symbol <= 'F') || symbol == '#')
                    return true;

                else return false;
            }

            foreach (char symbol in color[1..].ToUpper()) {
                if (!(('0' <= symbol && symbol <= '9') || ('A' <= symbol && symbol <= 'F'))) 
                    return false;
            }

            return true;
        }

        private bool _backgroundTransparency = false;

        public bool BackgroundTransparency
        {
            get { return _backgroundTransparency; }
            set
            {
                _backgroundTransparency = value;
                IsBackgroundEnabled = !value;
                OnPropertyChanged();
            }
        }

        private static readonly string s_initialDirectory = GetInitialDirectory();

        /// <summary>
        /// This method is used to get the initial save directiory.
        /// </summary>
        private static string GetInitialDirectory()
        {
            StringBuilder initialDirectory = new();
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;

            for (int i = 0; i < exeDirectory.Length; ++i)
            {
                if (exeDirectory[i..(i + 4)] == "bin\\") break;
                initialDirectory.Append(exeDirectory[i]);
            }

            initialDirectory.Append("Images/");

            string directory = initialDirectory.ToString();

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return directory;
        }

        private string _savePath = s_initialDirectory;

        public string SavePath
        {
            get { return _savePath; }
            set
            {
                _savePath = value;
                OnPropertyChanged();
            }
        }

        private bool _isPathAsked = false;

        public bool IsPathAsked
        {
            get { return _isPathAsked; }
            set
            {
                _isPathAsked = value;
                OnPropertyChanged();
            }
        }

        private ICommand? _selectSavePathCommand;

        public ICommand? SelectSavePathCommand
        {
            get
            {
                _selectSavePathCommand ??= new RelayCommand(execute => ExecuteSelectSavePathCommand(),
                    canExecute => CanExecuteSelectSavePathCommand());

                return _selectSavePathCommand;
            }
        }

        private void ExecuteSelectSavePathCommand()
        {
            using CommonOpenFileDialog fileDialog = new();
            fileDialog.IsFolderPicker = true;
            fileDialog.InitialDirectory = s_initialDirectory;

            if (fileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SavePath = fileDialog.FileName;
            }
            
        }

        private bool CanExecuteSelectSavePathCommand() => !IsPathAsked;

        private bool _isHighestResolution;

        public bool IsHighestResolution
        {
            get { return _isHighestResolution; }
            set
            {
                _isHighestResolution = value;
                OnPropertyChanged();
            }
        }

        private bool _isOptimized;

        public bool IsOptimized
        {
            get { return _isOptimized; }
            set
            {
                _isOptimized = value;
                OnPropertyChanged();
            }
        }

        private CorrectionLevel _selectedCorrectionLevel = CorrectionLevel.M;

        public CorrectionLevel SelectedCorrectionLevel
        {
            get { return _selectedCorrectionLevel; }
            set
            {
                _selectedCorrectionLevel = value;
                OnPropertyChanged();
            }
        }

        private readonly ObservableCollection<CorrectionLevel> _correctionLevels = new()
        {
            CorrectionLevel.L,
            CorrectionLevel.M,
            CorrectionLevel.Q,
            CorrectionLevel.H
        };

        public ObservableCollection<CorrectionLevel> CorrectionLevels
        {
            get { return _correctionLevels; }
        }

        private ICommand? _resetToDefaultsCommand;

        public ICommand? ResetToDefaultsCommand
        {
            get
            {
                _resetToDefaultsCommand ??= new RelayCommand(execute => ExecuteResetToDefaultsCommand());

                return _resetToDefaultsCommand;
            }
        }

        private void ExecuteResetToDefaultsCommand()
        {
            MessageBoxResult resetConfirmation = MessageBox.Show(
                "Are you sure you want to reset the settings?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (resetConfirmation == MessageBoxResult.Yes)
            {
                Background = "#FFFFFF";
                Foreground = "#000000";
                BackgroundTransparency = false;
                IsPathAsked = false;
                SavePath = s_initialDirectory;
                IsHighestResolution = false;
                IsOptimized = false;
                SelectedCorrectionLevel = CorrectionLevel.M;
            }
        }

        #endregion

        #region Control buttons

        private ICommand? _exitCommand;

        public ICommand? ExitCommand
        {
            get 
            {
                _exitCommand ??= new RelayCommand(execute => ExecuteExitCommand());

                return _exitCommand;
            }
        }

        private static void ExecuteExitCommand() => Application.Current.Shutdown();
        
        private ICommand? _minimizeCommand;

        public ICommand? MinimizeCommand
        {
            get
            {
                _minimizeCommand ??= new RelayCommand(execute => ExecuteMinimizeCommand());

                return _minimizeCommand;
            }
        }

        private static void ExecuteMinimizeCommand() => Application.Current.MainWindow.WindowState = WindowState.Minimized;


        private ICommand? _saveQRCommand;

        public ICommand? SaveQRCommand
        {
            get
            {
                _saveQRCommand ??= new RelayCommand(execute => ExecuteSaveCommand(),
                    canExecute => CanExecuteSaveCommand());

                return _saveQRCommand;
            }
        }

        private void ExecuteSaveCommand()
        {
            if (!IsPathAsked)
            {
                try
                {
                    string name = GenerateFileName();
                    using FileStream fileStream = new($"{SavePath}/{name}.png", FileMode.Create);
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(_bitmapImageSource));
                    encoder.Save(fileStream);

                    MessageBox.Show("QR was successfully saved.", "Success!", MessageBoxButton.OK);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                SaveFileDialog saveFileDialog = new();
                saveFileDialog.Filter = "Jpeg image |*.jpg|Png image|*.png|Bitmap image|*.bmp";

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        using var stream = saveFileDialog.OpenFile();
                        BitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(_bitmapImageSource));
                        encoder.Save(stream);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    
                }
            }
        }

        private bool CanExecuteSaveCommand() => _bitmapImageSource != null;

        /// <summary>
        /// This method is used to generate a name for a file.
        /// </summary>
        private static string GenerateFileName()
        {
            Random random = new();
            int nameLength = random.Next(8, 17);
            string fileName = string.Empty;
            string possibleCharacters = "abcdefghijklmnopqrtuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            while (fileName.Length < nameLength)
            {
                fileName += possibleCharacters[random.Next(0, possibleCharacters.Length)];
            }

            return fileName;
        }

        #endregion

        #region Input properties

        private int _selectedTabIndex = 0;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        #region URL

        private string _urlText = "https://";

        public string UrlText
        {
            get { return _urlText; }
            set
            {
                _urlText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Text

        private string _text = string.Empty;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                TextSymbolsCounter = _text.Length.ToString();
                OnPropertyChanged();
            }
        }

        private string _textSymbolsCounter = "0";

        public string TextSymbolsCounter
        {
            get { return _textSymbolsCounter; }
            set
            {
                _textSymbolsCounter = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Email

        private string _sendTo = "someone@email.com";

        public String SendTo
        {
            get { return _sendTo; }
            set
            {
                _sendTo = value;
                OnPropertyChanged();
            }
        }

        private string _subject = string.Empty;

        public string Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
                OnPropertyChanged();
            }
        }

        private string _emailText = string.Empty;

        public string EmailText
        {
            get { return _emailText; }
            set
            {
                _emailText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Location

        private string _latitude = string.Empty;

        public string Latitude
        {
            get { return _latitude; }
            set
            {
                if (VerifyCoordinate(value))
                {
                    _latitude = value;
                }

                OnPropertyChanged();
            }
        }

        private string _longitude = string.Empty;

        public string Longitude
        {
            get { return _longitude; }
            set
            {
                if (VerifyCoordinate(value))
                {
                    _longitude = value;
                }
                
                OnPropertyChanged();
            }
        }

        private static bool VerifyCoordinate(string coordinate)
        {
            if (coordinate.Count(f => f == '.') > 1) return false;
            if (coordinate.Count(f => f == '-') > 1) return false;

            string digits = coordinate.Replace(".", "").Replace("-", "");
            if (digits.Length == 0) return true;

            if (int.TryParse(digits, out _) is false) return false;

            return true;
        }

        #endregion

        #region Phone

        private string _phoneNumber = string.Empty;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (ValidatePhoneNumber(value))
                {
                    _phoneNumber = value;
                }
                
                OnPropertyChanged();
            }
        }

        private bool ValidatePhoneNumber(string number)
        {
            int plusCount = number.Count(f => f == '+');

            if (plusCount > 1) return false;
            if (plusCount == 1 && number.IndexOf('+') != 0) return false;

            string digits = number.Replace("+", "");
            if (digits.Length == 0) return true;

            if (long.TryParse(digits, out _) is false) return false; 

            return true;
        }

        #endregion

        #region WiFi

        private string _networkName = string.Empty;
        public string NetworkName
        {
            get { return _networkName; }
            set
            {
                _networkName = value;
                OnPropertyChanged();
            }
        }

        private readonly ObservableCollection<string> _networkTypes = new()
        {
            "WEP",
            "WPA",
            "WPA2-EAP",
            "No encryption"
        };

        public ObservableCollection<string> NetworkTypes 
        {
            get { return _networkTypes; }
        }

        private string _selectedNetworkType = "No encryption";

        public string SelectedNetworkType
        {
            get { return _selectedNetworkType; }
            set
            {
                _selectedNetworkType = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region Generation

        private BitmapSource? _bitmapImageSource = null;

        private ImageSource? _imageSource;

        public ImageSource? ImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }

        private string _data = string.Empty;

        // This dictionary contains the URI reserved characters and corresponding to them ASCII byte values
        private static readonly SortedDictionary<char, string> s_percentEncodingChars = new()
        {
            { ' ', "%20" }, { '!', "%21" }, { '"', "%22" },
            { '#', "%23" }, { '$', "%24" }, { '%', "%25" },
            { '&', "%26" }, { '\'', "%27" }, { '(', "%28" },
            { ')', "%29" }, { '*', "%30" }, { '+', "%31" },
            { ',', "%32" }, { '/', "%33" }, { ':', "%34" },
            { ';', "%35" }, { '=', "%36" }, { '?', "%37" },
            { '@', "%38" }, { '[', "%39" }, { ']', "%40" }
        };

        private ICommand? _generateCommand;

        public ICommand? GenerateCommand
        {
            get
            {
                _generateCommand ??= new RelayCommand(execute => ExecuteGenerateCommand(),
                    canExecute => CanGenerateCommand());
                
                return _generateCommand;
            }
        }

        private void ExecuteGenerateCommand()
        {
            SetupConfiguration();

            try
            {
                DataEncoding.Encode(_data);
                ServiceInformation.AddServiceInformation();
                SeparationIntoBlocks.Separate();
                CorrectionBytesCreation.CreateCorrectionBytes();
                CombiningBlocks.Combine();
                InformationPlacement.PlaceInformation();
                QRCodeRendering.CreateQR();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _bitmapImageSource = Imaging.CreateBitmapSourceFromHBitmap(
                QRCodeRendering.BitmapImage.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
            );

            ImageSource = _bitmapImageSource;
        }

        private bool CanGenerateCommand()
        {
            if (!(VerifyColor(Foreground))) 
                return false;

            else if (!BackgroundTransparency)
            {
                if (!VerifyColor(Background)) 
                    return false;
            }

            switch (SelectedTabIndex)
            {
                case 0:
                {
                    if (UrlText.Length == 0)
                    {
                        return false;
                    }
                    break;
                }
                case 1:
                {
                    if (Text.Length == 0) return false;
                    break;
                }
                case 2:
                {
                    if (SendTo.Length == 0 || Subject.Length == 0 || EmailText.Length == 0) return false;
                    break;
                }
                case 3:
                {
                    if (Latitude.Length == 0 || Longitude.Length == 0) return false;
                    break;
                }
                case 4:
                {
                    if (PhoneNumber.Length == 0) return false;
                    break;
                }
                case 5:
                {
                    if (NetworkName.Length == 0) return false;
                    if (SelectedNetworkType != "No encryption" && Password.Length == 0) return false;
                    break;
                }
                default: 
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This method is used to set up the model for a proper functioning.
        /// </summary>
        private void SetupConfiguration()
        {
            switch (SelectedTabIndex)
            {
                case 0:
                {
                    _data = ReplaceSpaces(UrlText);
                    if (!_data.StartsWith("https://")) _data = $"https://{_data}";
                    break;
                }

                case 1:
                {
                    _data = Text;
                    break;
                }
                case 2:
                {
                    string subject = ReplaceNotAlphanumericChars(Subject);
                    string body = ReplaceNotAlphanumericChars(EmailText);
                    _data = $"mailto:{SendTo}?subject={subject}&body={body}";
                    break;
                }
                case 3:
                {
                    _data = $"geo:{Latitude},{Longitude}";
                    break;
                }
                case 4:
                {
                    _data = $"tel:{PhoneNumber}";
                    break;
                }
                case 5:
                {
                    string networkType;

                    if (SelectedNetworkType.Equals("No encryption"))
                        networkType = "nopass";
                    else 
                        networkType = SelectedNetworkType;

                    _data = $"WIFI:T:{networkType};S:{NetworkName};P:{Password};;";
                    break;
                }
            }

            Configuration.EncodingMethod = GetEncodingMethod(_data);

            switch (SelectedCorrectionLevel)
            {
                case CorrectionLevel.L: { Configuration.CorrectionLevel = CorrectionLevel.L; break; }
                case CorrectionLevel.M: { Configuration.CorrectionLevel = CorrectionLevel.M; break; }
                case CorrectionLevel.Q: { Configuration.CorrectionLevel = CorrectionLevel.Q; break; }
                case CorrectionLevel.H: { Configuration.CorrectionLevel = CorrectionLevel.H; break; }
            }

            if (BackgroundTransparency)
            {
                Configuration.Background = System.Drawing.Color.Transparent;
            }
            else
            {
                Configuration.Background = ConvertStringToSystemDrawingColor(Background);
            }

            Configuration.Foreground = ConvertStringToSystemDrawingColor(Foreground);
            Configuration.IsHighestResolution = IsHighestResolution;
            Configuration.IsOptimized = IsOptimized;
        }

        /// <summary>
        /// This method is used to get the encoding method based on the entered data
        /// </summary>
        private static EncodingMethod GetEncodingMethod(string data)
        {
            EncodingMethod encodingMethod = EncodingMethod.Numeric;

            foreach (char symbol in data)
            {
                if (char.IsDigit(symbol)) continue;
                else if ( (65 <= symbol && symbol <= 90) ||
                    symbol == ' ' || symbol == '$' || symbol == '%' ||
                    symbol == '*' || symbol == '+' || symbol == '-' ||
                    symbol == '.' || symbol == '/' || symbol == ':')
                {
                    encodingMethod = EncodingMethod.Alphanumeric;
                }
                else
                {
                    encodingMethod = EncodingMethod.Binary;
                    break;
                }
            }

            return encodingMethod;
        }

        /// <summary>
        /// This method is used to replace all reserved characters with their ASCII byte values for
        /// a proper encoding
        /// </summary>
        private static string ReplaceNotAlphanumericChars(string data)
        {
            StringBuilder sb = new();

            foreach (char symbol in data)
            {
                s_percentEncodingChars.TryGetValue(symbol, out string? value);

                if (value is null) sb.Append(symbol);
                else sb.Append(value);
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method is used to replace all the spaces with their ASCII byte value
        /// </summary>
        private static string ReplaceSpaces(string data)
        {
            StringBuilder sb = new();

            foreach(char symbol in data)
            {
                if (symbol == ' ') sb.Append("%20");
                else sb.Append(symbol);
            }

            return sb.ToString();
        }

        /// <summary>
        /// This method is used to convert a color (System.Drawing.Color) from its string hex representation.
        /// </summary>
        private static System.Drawing.Color ConvertStringToSystemDrawingColor(string color)
        {
            byte r = 0, g = 0, b = 0;

            if (color.Length == 7)
            {
                r = Convert.ToByte(color[1..3], 16);
                g = Convert.ToByte(color[3..5], 16);
                b = Convert.ToByte(color[5..7], 16);
            }
            else if (color.Length == 4)
            {
                r = Convert.ToByte(color[1..2] + color[1..2], 16);
                g = Convert.ToByte(color[2..3] + color[2..3], 16);
                b = Convert.ToByte(color[3..4] + color[3..4], 16);
            }

            return System.Drawing.Color.FromArgb(r, g, b);
        }

        #endregion

        #region INotifyPropertyChanged interface implementation

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName]string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
