using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeeslagFrontEnd.Shared.Grid
{
    public class StyleGrid
    {
        private string _emptyStyle = "cell-empty";
        private string _missStyle = "cell-miss";
        private string _hitStyle = "cell-hit";

        private int _size;

        public string[,] Grid { get; private set; }

        public StyleGrid(int size)
        {
            _size = size;
            Grid = new string[size,size];
            SetDefault();
        }

        public void SetMiss(int x, int y)
        {
            Grid[x, y] = _missStyle;
        }

        public void SetHit(int x, int y)
        {
            Grid[x, y] = _hitStyle;
        }

        private void SetDefault()
        {
            for (int i = 0; i < _size; i++)
            {
                for(int j = 0; j < _size; j++)
                {
                    Grid[i, j] = _emptyStyle;
                }
            }
        }
    }
}
