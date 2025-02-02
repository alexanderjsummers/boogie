using System.Collections.Generic;

namespace Microsoft.Boogie
{
    class GlobalSnapshotInstrumentation
    {
        private Dictionary<Variable, Variable> oldGlobalMap;
        private List<Variable> newLocalVars;
        
        public GlobalSnapshotInstrumentation(CivlTypeChecker civlTypeChecker)
        {
            newLocalVars = new List<Variable>();
            oldGlobalMap = new Dictionary<Variable, Variable>();
            foreach (Variable g in civlTypeChecker.sharedVariables)
            {
                LocalVariable l = OldGlobalLocal(g);
                oldGlobalMap[g] = l;
                newLocalVars.Add(l);
            }
        }

        public Dictionary<Variable, Variable> OldGlobalMap => oldGlobalMap;

        public List<Variable> NewLocalVars => newLocalVars;

        public List<Cmd> CreateUpdatesToOldGlobalVars()
        {
            List<AssignLhs> lhss = new List<AssignLhs>();
            List<Expr> rhss = new List<Expr>();
            foreach (Variable g in oldGlobalMap.Keys)
            {
                lhss.Add(new SimpleAssignLhs(Token.NoToken, Expr.Ident(oldGlobalMap[g])));
                rhss.Add(Expr.Ident(g));
            }
            var cmds = new List<Cmd>();
            if (lhss.Count > 0)
            {
                cmds.Add(new AssignCmd(Token.NoToken, lhss, rhss));
            }
            return cmds;
        }

        public List<Cmd> CreateInitCmds()
        {
            List<AssignLhs> lhss = new List<AssignLhs>();
            List<Expr> rhss = new List<Expr>();
            foreach (Variable g in oldGlobalMap.Keys)
            {
                lhss.Add(new SimpleAssignLhs(Token.NoToken, Expr.Ident(oldGlobalMap[g])));
                rhss.Add(Expr.Ident(g));
            }
            var initCmds = new List<Cmd>();
            if (lhss.Count > 0)
            {
                initCmds.Add(new AssignCmd(Token.NoToken, lhss, rhss));
            }
            return initCmds;
        }

        public List<Cmd> CreateAssumeCmds()
        {
            List<Cmd> newCmds = new List<Cmd>();
            foreach (Variable v in oldGlobalMap.Keys)
            {
                newCmds.Add(new AssumeCmd(Token.NoToken, Expr.Eq(Expr.Ident(v), Expr.Ident(oldGlobalMap[v]))));
            }
            return newCmds;
        }
        
        private LocalVariable OldGlobalLocal(Variable v)
        {
            return new LocalVariable(Token.NoToken,
                new TypedIdent(Token.NoToken, $"og_global_old_{v.Name}", v.TypedIdent.Type));
        }
    }
}
