using GameLogic;
using GameLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window
    {
        public StartWindow()
        {
            InitializeComponent();

            StartGame(PieceColor.White); // color should be selected by host and obtained from server for opponent
            //Visibility = Visibility.Hidden;

        }


        private void StartGame(PieceColor playerColor)
        {
            Board board = new();
            board.Initialize();
            GameManager gameManager = new(board, playerColor); 

            GameWindow gameWindow = new(gameManager);
            gameWindow.Show();
            Close();
        }
    }
}
