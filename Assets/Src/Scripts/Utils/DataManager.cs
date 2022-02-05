using UnityEngine;

namespace YsoCorp {

    [DefaultExecutionOrder(-1)]
    public class DataManager : ADataManager {

        private static string PSEUDO = "PSEUDO";
        private static string LEVEL = "LEVEL";
        private static string LEVEL_MAX = "LEVEL_MAX";
        private static string STAR_NB = "STAR_NB";
        private static string STAR_BY_LEVELS = "STAR_BY_LEVELS"; 
        private static string NUMCHARACTER = "NUMCHARACTER";

        private static int DEFAULT_LEVEL = 1;

        /***** CUSTOM  *****/

        // LEVEL
        public int GetLevel() {
            return this.GetInt(LEVEL, DEFAULT_LEVEL);
        }
        public int GetLevelMax() {
            return this.GetInt(LEVEL_MAX, DEFAULT_LEVEL);
        }
        public int SetLevel(int newLevel) {
            this.SetInt(LEVEL, newLevel);
            return newLevel;
        }
        public int NextLevel() {
            int level = this.GetLevel() + 1;
            this.SetInt(LEVEL, this.GetLevel() + 1);
            if (level > this.GetLevelMax())
                this.SetInt(LEVEL_MAX, this.GetLevelMax() + 1);
            return level;
        }
        public int PrevLevel() {
            int level = Mathf.Max(this.GetLevel() - 1, DEFAULT_LEVEL);
            this.SetInt(LEVEL, level);
            return level;
        }

        //STAR
        public int GetStarNb() {
            return this.GetInt(STAR_NB, 0);
        }
        public string GetStarByLevels() {
            return this.GetString(STAR_BY_LEVELS, "000000000000000000000000000000");
        }

        public int AddStar() {
            this.SetInt(STAR_NB, this.GetStarNb() + 1);
            return this.GetStarNb();
        }

        public string SetStarByLevels(string newStarByLevels) {
            this.SetString(STAR_BY_LEVELS, newStarByLevels);
            return newStarByLevels;
        }

        public bool StarInLevelIsTaken(int level) {
            string starStr = this.GetStarByLevels();

            if (level >= starStr.Length)
                return true;
            
            return starStr[level] == '1';
        }

        public string updateStarOnLevel(int level) {
            string starStr = this.GetStarByLevels();
            string tmp = "";

            if (level >= starStr.Length)
                return starStr;
            else if (starStr[level - 1] == '1')
                return starStr;

            this.AddStar();
            for (int i = 0; i < starStr.Length; i++) {
                if (i != level - 1)
                    tmp += starStr[i];
                else
                    tmp += '1';
            }

            return this.SetStarByLevels(tmp);
        }

        //PLAYER NAME
        public string GetPseudo() {
            return this.GetString(PSEUDO, "Player");
        }
        public void SetPseudo(string pseudo) {
            this.SetString(PSEUDO, pseudo);
        }

        // NUM CHARACTER
        public int GetNumCharacter() {
            return this.GetInt(NUMCHARACTER, -1);
        }
        public void SetNumCharacter(int num) {
            this.SetInt(NUMCHARACTER, num);
        }
        public void UnlockNumCharacter(int num) {
            this.SetInt(NUMCHARACTER + num, 1);
        }
        public bool IsUnlockNumCharacter(int num) {
            this.UnlockNumCharacter(0);
            return this.GetInt(NUMCHARACTER + num, 0) == 1;
        }

    }
}