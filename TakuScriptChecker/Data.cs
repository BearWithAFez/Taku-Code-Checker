using System.Collections.Generic;

namespace TakuCodeChecker {
    public class Data {
        public string Name;
        public string Author;
        private List<Diff> _Diff = new List<Diff>();

        public List<Diff> Diff {
            get { return _Diff; }
            set { _Diff = value; }
        }
    }

    public class Diff {
        public string Name;
        private List<Puzzle> _Puzzle = new List<Puzzle>();

		public List<Puzzle> Puzzle {
			get { return _Puzzle; }
			set { _Puzzle = value; }
        }
    }

    public class Puzzle {
        public int Id;
        public string Code;
    }
}