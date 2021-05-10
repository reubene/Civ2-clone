namespace Civ2engine
{
    public class LeaderDefaults
    {
        public string NameMale { get; set; }
        public string NameFemale { get; set; }
        public int Female { get; set; }
        public int Color { get; set; }
        public int CityStyle { get; set; }
        public string Plural { get; set; }
        public string Adjective { get; set; }
        public int Attack { get; set; }
        public int Expand { get; set; }
        public int Civilize { get; set; }

        public LeaderTitle[] Titles { get; set; }
    }
}