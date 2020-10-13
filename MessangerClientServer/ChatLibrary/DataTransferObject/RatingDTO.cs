using System;

namespace ChatLibrary.DataTransferObject
{
    [Serializable]
    public class RatingDTO
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public int CountAllGame { get; set; }

        public int CountWin { get; set; }

        public int CountDraw { get; set; }

        public int CountLose { get; set; }

        public string PercentOfWin => CountAllGame != 0 ? $"{(float) CountWin / CountAllGame * 100:N2}" : "N/A";
    }
}