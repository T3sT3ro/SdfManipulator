using System.Text;

namespace AST.Syntax {
    public interface IWriteable {
        public StringBuilder WriteTo(StringBuilder sb);
    }
}
