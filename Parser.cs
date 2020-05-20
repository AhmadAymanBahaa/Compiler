using System;
using System.Collections.Generic;

namespace ScannerProject
{
    public enum NodeKind { StmtK, ExpK };
    public enum StmtKind { IfK, RepeatK, AssignK, ReadK, WriteK };
    public enum ExpKind { OpK, ConstK, IdK };
    public enum ExpType { Void, Integer, Boolean };

    class Parser
    {
        static FileReader fd = new FileReader();
        Scanner S = new Scanner(fd);
        public const int MAXCHILDREN = 3;

        public KeyValuePair<string, TokenType> token;
        public int currentTokenNumber = 0;

        public Parser(FileReader path) {
            fd = path;
        }

        public List<KeyValuePair<string, TokenType>> ScanFile() {
            return S.scan();
        }

        public class TreeNode
        {
            public List<TreeNode> child = new List<TreeNode>();
            public TreeNode sibling = new TreeNode();
            public NodeKind nodekind;

            public class Kind { public StmtKind stmt; public ExpKind exp; }
            public Kind kind = new Kind();

            public class Attr { public TokenType op; public int val; public string name; }
            public Attr attr = new Attr();
            public ExpType type; /* for type checking of exps */
        }

        TreeNode newStmtNode(StmtKind kind)
        {
            TreeNode t = new TreeNode();
            int i;
            if (t == null)
                Console.Write("Out of memory error at line %d\n");
            else
            {
                for (i = 0; i < MAXCHILDREN; i++) t.child[i] = null;
                t.sibling = null;
                t.nodekind = NodeKind.StmtK;
                t.kind.stmt = kind;
            }
            return t;
        }

        TreeNode newExpNode(ExpKind kind)
        {
            TreeNode t = new TreeNode();
            int i;
            if (t == null)
                Console.Write("Out of memory error at line %d\n");
            else
            {
                for (i = 0; i < MAXCHILDREN; i++) t.child[i] = null;
                t.sibling = null;
                t.nodekind = NodeKind.ExpK;
                t.kind.exp = kind;
                t.type = ExpType.Void;
            }
            return t;
        }

        void match(TokenType expected)
        {
            if (token.Value == expected) token = getToken();
            else
            {
                /* syntaxError("unexpected token -> ");
                 printToken(token, tokenString);
                 fprintf(listing, "      ");*/
                Console.Write("Unexpected Token");
            }
        }

        public KeyValuePair<string, TokenType> getToken()
        {
            return S.ScannedList[currentTokenNumber++];
        }


        public TreeNode stmt_sequence()
        {
            TreeNode t = statement();
            TreeNode p = t;
            //add end token 
            while ((token.Value != TokenType.ENDFILE) && (token.Value != TokenType.T_END) &&
                   (token.Value != TokenType.T_ELSE) && (token.Value != TokenType.T_UNTIL))
            {
                TreeNode q;
                match(TokenType.T_SEMICOLON);
                q = statement();
                if (q != null)
                {
                    if (t == null) t = p = q;
                    else /* now p cannot be null either */
                    {
                        p.sibling = q;
                        p = q;
                    }
                }
            }
            return t;
        }

        TreeNode statement()
        {
            TreeNode t = null;
            switch (token.Value)
            {
                case TokenType.T_IF: t = if_stmt(); break;
                case TokenType.T_REPEAT: t = repeat_stmt(); break;
                case TokenType.ID: t = assign_stmt(); break;
                case TokenType.T_READ: t = read_stmt(); break;
                case TokenType.T_WRITE: t = write_stmt(); break;
                default:
                    //syntaxError("unexpected token -> ");
                    //printToken(token, tokenString);
                    Console.WriteLine(token);
                    token = getToken();
                    break;
            } /* end case */
            return t;
        }


        TreeNode if_stmt()
        {
            TreeNode t = newStmtNode(StmtKind.IfK);
            match(TokenType.T_IF);
            if (t != null) t.child[0] = exp();
            match(TokenType.T_THEN);
            if (t != null) t.child[1] = stmt_sequence();
            if (token.Value == TokenType.T_ELSE)
            {
                match(TokenType.T_ELSE);
                if (t != null) t.child[2] = stmt_sequence();
            }
            match(TokenType.T_END);
            return t;
        }

        TreeNode repeat_stmt()
        {
            TreeNode t = newStmtNode(StmtKind.RepeatK);
            match(TokenType.T_REPEAT);
            if (t != null) t.child[0] = stmt_sequence();
            match(TokenType.T_UNTIL);
            if (t != null) t.child[1] = exp();
            return t;
        }

        TreeNode assign_stmt()
        {
            TreeNode t = newStmtNode(StmtKind.AssignK);
            if ((t != null) && (token.Value == TokenType.ID))
                //    t.attr.name = copyString(tokenString);
                match(TokenType.ID);
            match(TokenType.T_ASSIGN);
            if (t != null) t.child[0] = exp();
            return t;
        }

        TreeNode read_stmt()
        {
            TreeNode t = newStmtNode(StmtKind.ReadK);
            match(TokenType.T_READ);
            if ((t != null) && (token.Value == TokenType.ID))
                // t.attr.name = copyString(tokenString);
                match(TokenType.ID);
            return t;
        }

        TreeNode write_stmt()
        {
            TreeNode t = newStmtNode(StmtKind.WriteK);
            match(TokenType.T_WRITE);
            if (t != null) t.child[0] = exp();
            return t;
        }

        TreeNode exp()
        {
            TreeNode t = simple_exp();
            if ((token.Value == TokenType.T_LESSTHAN) || (token.Value == TokenType.T_EQUALS))
            {
                TreeNode p = newExpNode(ExpKind.OpK);
                if (p != null)
                {
                    p.child[0] = t;
                    p.attr.op = token.Value;
                    t = p;
                }
                match(token.Value);
                if (t != null)
                    t.child[1] = simple_exp();
            }
            return t;
        }
        TreeNode simple_exp()
        {
            TreeNode t = term();
            while ((token.Value == TokenType.T_PLUS) || (token.Value == TokenType.T_MINUS))
            {
                TreeNode p = newExpNode(ExpKind.OpK);
                if (p != null)
                {
                    p.child[0] = t;
                    p.attr.op = token.Value;
                    t = p;
                    match(token.Value);
                    t.child[1] = term();
                }
            }
            return t;
        }

        TreeNode term()
        {
            TreeNode t = factor();
            while ((token.Value == TokenType.T_TIMES) || (token.Value == TokenType.T_OVER))
            {
                TreeNode p = newExpNode(ExpKind.OpK);
                if (p != null)
                {
                    p.child[0] = t;
                    p.attr.op = token.Value;
                    t = p;
                    match(token.Value);
                    p.child[1] = factor();
                }
            }
            return t;
        }

        TreeNode factor()
        {
            TreeNode t = null;
            switch (token.Value)
            {
                case TokenType.NUMBER:
                    t = newExpNode(ExpKind.ConstK);
                    if ((t != null) && (token.Value == TokenType.NUMBER))
                        t.attr.val = int.Parse(token.Key);
                    match(TokenType.NUMBER);
                    break;
                case TokenType.ID:
                    t = newExpNode(ExpKind.IdK);
                    if ((t != null) && (token.Value == TokenType.ID))
                        // t.attr.name = copyString(S.tokenString);
                        match(TokenType.ID);
                    break;
                case TokenType.T_LEFTPAREN:
                    match(TokenType.T_LEFTPAREN);
                    t = exp();
                    match(TokenType.T_RIGHTPAREN);
                    break;
                default:
                    // syntaxError("unexpected token -> ");
                    //printToken(token, S.tokenString);
                    Console.WriteLine(token);
                    token = getToken();
                    break;
            }
            return t;
        }

    }
}