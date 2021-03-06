﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace μScore
{
    public partial class FavoriteControl : UserControl
    {
        private main Main;

        public FavoriteControl(main m)
        {
            InitializeComponent();
            Main = m;
            getScore();
        }

        //得到当前用户的Score表数据并添加到表格中
        public void getScore()
        {
            scoreDataGrid.Rows.Clear();
            string selectAll =
                "SELECT scoreTitle, musicianName, sourceTitle, albumTitle, createDate " +
                "FROM favorite " +
                "join score on favorite.scoreID=score.scoreID " +
                "join musician on score.musicianID = musician.musicianID " +
                "join source on score.sourceID = source.sourceID " +
                "join album on score.albumID = album.albumID " +
                "where userID= '" + Sign.userID + "' " +
                "order by addDateTime desc;";

            using (MySqlCommand cmd = new MySqlCommand(selectAll, Sign.conn))
            {
                MySqlDataReader myReader = null;
                int index = 0;
                try
                {
                    myReader = cmd.ExecuteReader();
                    if (!myReader.HasRows)
                    {
                        infoLabel.Text = "Sorry, no music score has been add to your favorite";
                        infoLabel.Location = new Point(490 - infoLabel.Width / 2, 382);
                        return;
                    }
                    else
                        infoLabel.Text = "";

                    while (myReader.Read())
                    {
                        if (myReader.HasRows)
                        {
                            DataGridViewRow row = new DataGridViewRow();
                            scoreDataGrid.Rows.Add(row);
                            for (int i = 0; i < 5; i++)
                            {
                                scoreDataGrid.Rows[index].Height = 30;
                                scoreDataGrid.Rows[index].Cells[i].Value = myReader.GetString(i);
                            }
                            index++;
                        }
                    }
                    scoreDataGrid[0, 0].Selected = false;
                }
                catch (MySqlException exp)
                {
                    MessageBox.Show(exp.ToString());
                }
                finally
                {
                    myReader.Close();
                }
            }
        }

        //双击表格跳转到对应的Score
        private void scoreDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            int scoreID = 0;
            string getScoreID = "select scoreID " +
                                "from score " +
                                "join musician on score.musicianID=musician.musicianID " +
                                "where scoreTitle='" + scoreDataGrid.Rows[e.RowIndex].Cells[0].Value.ToString().Replace("'", "''") + "'" +
                                "and musicianName='" + scoreDataGrid.Rows[e.RowIndex].Cells[1].Value.ToString().Replace("'", "''") + "';";
            using (MySqlCommand cmd = new MySqlCommand(getScoreID, Sign.conn))
            {
                MySqlDataReader myReader = null;
                try
                {
                    myReader = cmd.ExecuteReader();
                    myReader.Read();
                    if (myReader.HasRows)
                    {
                        scoreID = myReader.GetInt32(0);
                    }
                }
                catch (MySqlException exception)
                {
                    MessageBox.Show(exception.ToString());
                    return;
                }
                finally
                {
                    myReader.Close();
                }
            }

            main.Score.showScore(scoreID);
            Main.changePanel(main.Score);
        }

        //部件换色
        public void changeColor()
        {
            skinFlowLayoutPanel1.BackColor = Theme.MainColor1;
            infoLabel.ForeColor = Theme.MainColor1;
        }
    }
}
