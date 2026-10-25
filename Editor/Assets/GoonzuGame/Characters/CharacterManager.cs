using System;

namespace GoonzuGame.Characters
{
    public class CharacterManager
    {
        public static CharacterManager Instance { get; } = new CharacterManager();
        public Character PlayerCharacter { get; set; }

        private CharacterManager() {}
    }
}