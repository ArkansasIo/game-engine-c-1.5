using System;
using System.Collections.Generic;

namespace GoonzuGame.DnD5e
{
    public static class CoreMechanics
    {
        public static int RollDice(int sides) => new Random().Next(1, sides + 1);
        public static int RollD20() => RollDice(20);
        public static int AbilityModifier(int score) => (score - 10) / 2;
        public static int ProficiencyBonus(int level) => 2 + ((level - 1) / 4);
        public static int DC(int baseDC, int mod) => baseDC + mod;
        public static int PassiveScore(int score, int mod) => 10 + AbilityModifier(score) + mod;
        public static int ContestedRoll(int a, int b) => a > b ? a : b;
        public static int Advantage(int roll1, int roll2) => Math.Max(roll1, roll2);
        public static int Disadvantage(int roll1, int roll2) => Math.Min(roll1, roll2);
    }
}