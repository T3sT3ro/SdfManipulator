using System.Text;

namespace me.tooster.sdf.AST.Syntax {
    public interface IWriteable {
        public StringBuilder WriteTo(StringBuilder sb);
    }
}
