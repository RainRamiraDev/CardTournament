namespace CTDto.Users.Player
{
    public class PlayerDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Alias { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int TournamentsWon { get; set; }
        public int Disqualifications { get; set; }
        public string DiscualificationsReasons { get; set; }
        public string AvatarUrl { get; set; }
    }
}
