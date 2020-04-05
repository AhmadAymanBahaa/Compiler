using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ScannerCode
{
    public class Scanner
    {
        //File to be scanned
        FileReader fd = new FileReader();

        //Class Constructor to Asssign the FileReader
        public Scanner(FileReader fd)
        {
            this.fd = fd;
        }

        public enum TokenType
        {
            //Reserved Words
            T_IF, T_THEN, T_ELSEIF, T_FLOAT, T_STRING,
            T_INT, T_RETURN,
            T_WRITE, T_END,
            T_ELSE, T_ENDL, T_REPEAT, T_UNTIL, T_READ,


            //One Character Tokens
            T_ASSIGN, T_EQUALS, T_LESSTHAN, T_LARGERTHAN,
            T_PLUS, T_MINUS, T_TIMES, T_OVER, T_LEFTPAREN, T_RIGHTPAREN,
            T_SEMICOLON, T_LBRACES, T_RBRACES, T_COMMA,

            ERROR, NUMBER, ID, AND, OR,

        }

        public struct identifier
        {
            TokenType ID;
            string value;
        }

        public enum StateType
        {
            START, INASSIGN, INCOMMENT, INNUM, INID, DONE, COMMENTDIV, ENDCOMMENT, INDECIMAL, ID, STRING, ANDLOGIC, ORLOGIC
        }

        //Hashtable for reserved words lexemes & tokens
        static Hashtable ReservedWords = new Hashtable()
        {
            { "if", TokenType.T_IF },
            { "then", TokenType.T_THEN },
            { "float", TokenType.T_FLOAT },
            { "string", TokenType.T_STRING },
            { "int", TokenType.T_INT },
            { "write", TokenType.T_WRITE },
            { "read", TokenType.T_READ },
            { "repeat", TokenType.T_REPEAT },
            { "until", TokenType.T_UNTIL },
            { "elseif", TokenType.T_ELSEIF },
            { "else", TokenType.T_ELSE },
            { "return", TokenType.T_RETURN },
            { "endl", TokenType.T_ENDL },
            { "end", TokenType.T_END },

        };

        //List to store scanned lexems and tokens
        //static Hashtable ScannedHashtable = new Hashtable() { };
        List<KeyValuePair<string, TokenType>> ScannedList = new List<KeyValuePair<string, TokenType>>();

        public TokenType ReservedWordsLookup(string lexeme)
        {
            if (ReservedWords.ContainsKey(lexeme))
            {
                return (TokenType)(ReservedWords[lexeme]);
            }
            return TokenType.ID;
        }


        void getToken(FileReader fd)
        {  /* index for storing into tokenString */
            int tokenStringIndex = 0;
            char[] tokenString = new char[40];
            //Holds current token
            TokenType currentToken = TokenType.ERROR;
            /* current state - always begins at START */
            StateType state = StateType.START;
            /* flag to indicate save to tokenString */
            Boolean save;

            while (state != StateType.DONE)
            {
                if (!(fd.lineno < fd.lines))
                {
                    currentToken = TokenType.T_ENDL;
                    break;
                }
                char c = fd.getNextChar();
                save = true;
                switch (state)
                {
                    case StateType.START:
                        if (char.IsDigit(c))
                            state = StateType.INNUM;
                        else if (char.IsLetter(c))
                            state = StateType.INID;
                        else if (c == ':')
                            state = StateType.INASSIGN;
                        else if ((c == ' ') || (c == '\t') || (c == '\n'))
                            save = false;
                        else if (c == '"')
                            state = StateType.STRING;
                        else if (c == '/')
                        {
                            state = StateType.COMMENTDIV;
                        }
                        else if (c == '&')
                        {
                            state = StateType.ANDLOGIC;
                        }
                        else if (c == '|')
                        {
                            state = StateType.ORLOGIC;
                        }
                        else
                        {
                            state = StateType.DONE;
                            switch (c)
                            {
                                case '=':
                                    currentToken = TokenType.T_EQUALS;
                                    break;
                                case '<':
                                    currentToken = TokenType.T_LESSTHAN;
                                    break;
                                case '>':
                                    currentToken = TokenType.T_LARGERTHAN;
                                    break;
                                case '+':
                                    currentToken = TokenType.T_PLUS;
                                    break;
                                case '-':
                                    currentToken = TokenType.T_MINUS;
                                    break;
                                case '*':
                                    currentToken = TokenType.T_TIMES;
                                    break;
                                case '(':
                                    currentToken = TokenType.T_LEFTPAREN;
                                    break;
                                case ')':
                                    currentToken = TokenType.T_RIGHTPAREN;
                                    break;
                                case ',':
                                    currentToken = TokenType.T_COMMA;
                                    break;
                                case '{':
                                    currentToken = TokenType.T_LBRACES;
                                    break;
                                case '}':
                                    currentToken = TokenType.T_RBRACES;
                                    break;
                                case ';':
                                    currentToken = TokenType.T_SEMICOLON;
                                    break;
                                default:
                                    currentToken = TokenType.ERROR;
                                    break;
                            }
                        }
                        break;
                    case StateType.INCOMMENT:
                        save = false;
                        if (c == '*') state = StateType.ENDCOMMENT;
                        break;
                    case StateType.COMMENTDIV:
                        if (c == '*')
                        {
                            save = false;
                            state = StateType.INCOMMENT;
                            tokenStringIndex--;
                        }
                        else
                        {
                            currentToken = TokenType.T_OVER;
                            save = false;
                            fd.linepos--;
                            state = StateType.DONE;
                        }
                        break;
                    case StateType.ENDCOMMENT:
                        if (c == '/')
                        {
                            save = false;
                            state = StateType.START;

                        }
                        else
                            state = StateType.INCOMMENT;
                        break;
                    case StateType.ANDLOGIC:
                          state = StateType.DONE;
                        if (c == '&')
                        {
                            currentToken = TokenType.AND;
                        }
                        else
                        {
                            fd.linepos--;
                            save = false;
                            currentToken = TokenType.ERROR;
                        }
                        break;
                    case StateType.ORLOGIC:
                         state = StateType.DONE;
                        if (c == '|')
                        {
                            currentToken = TokenType.OR;
                        }
                        else
                        {
                            fd.linepos--;
                            save = false;
                            currentToken = TokenType.ERROR;
                        }
                        break;
                    case StateType.STRING:
                        if (c == '"')
                        {
                            state = StateType.DONE;
                            currentToken = TokenType.T_STRING;
                        }
                        break;
                    case StateType.INASSIGN:
                        state = StateType.DONE;
                        if (c == '=')
                            currentToken = TokenType.T_ASSIGN;
                        else
                        { /* backup in the input */
                            fd.linepos--;
                            save = false;
                            currentToken = TokenType.ERROR;
                        }
                        break;
                    case StateType.INNUM:
                        if (c == '.')
                        {
                            state = StateType.INDECIMAL;
                        }
                        else if (!char.IsDigit(c))
                        { /* backup in the input */
                            fd.linepos--;
                            save = false;
                            state = StateType.DONE;
                            currentToken = TokenType.NUMBER;
                        }
                        break;
                    case StateType.INDECIMAL:
                        if (!char.IsDigit(c))
                        { /* backup in the input */
                            fd.linepos--;
                            save = false;
                            state = StateType.DONE;
                            currentToken = TokenType.NUMBER;
                        }
                        break;
                    case StateType.INID:
                        if (char.IsDigit(c))
                        {
                            state = StateType.ID;
                        }
                        else if (!char.IsLetter(c))
                        { /* backup in the input */
                            fd.linepos--;
                            save = false;
                            state = StateType.DONE;
                            currentToken = ReservedWordsLookup(new string(tokenString).Trim('\0'));
                        }
                        break;
                    case StateType.ID:
                        if (!(char.IsLetter(c) || char.IsDigit(c)))
                        { /* backup in the input */
                            fd.linepos--;
                            save = false;
                            currentToken = TokenType.ID;
                            state = StateType.DONE;

                        }
                        break;

                    case StateType.DONE:
                    default: /* should never happen */
                        Console.Write("Scanner Bug: state= %d\n", state);
                        state = StateType.DONE;
                        currentToken = TokenType.ERROR;
                        break;
                }
                if (save)
                    tokenString[tokenStringIndex++] = c;
                if (state == StateType.DONE)
                {
                    addScanned(tokenString, currentToken);
                }
            }

        }

        //add each lexeme scanned to Scanned hashtable
        void addScanned(char[] tokenString, TokenType token)
        {
            //ScannedHashtable.Add(new string(tokenString), token);
            ScannedList.Add(new KeyValuePair<string, TokenType>(new string(tokenString), token));
        }

        public void scanAndPrint()
        {
            while (fd.lineno < fd.lines)
            {
                getToken(fd);
            }
            printScanned();
        }

        //Print scanned lexemes and their tokens
        void printScanned()
        {
            Console.WriteLine("Lexeme              :  Token \n");
            foreach (var lexeme in ScannedList)
            {
                Console.WriteLine(String.Format("{0}:  {1}", lexeme.Key, lexeme.Value));
            }
        }

    }

    public class FileReader
    {
        public int lines; // total number of lines in a file
        string[] s;
        public int linepos; // current character
        public int lineno; // current line
        char[] char_arr;


        public FileReader()
        {
            linepos = -1;
            lineno = 0;
        }

        public void readAllFile(string filepath)
        {
            var fileStream = new FileStream(@filepath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream))

            {
                string line;
                string fileName = @filepath;
                lines = File.ReadAllLines(fileName).Length; //count no. of lines in a file
                s = new string[lines]; //declaring a string array of no. of lines' size
                int i = 0;
                //read all file
                while ((line = streamReader.ReadLine()) != null)
                {
                    s[i] = line;
                    s[i] += '\n';
                    i++;
                }

            }
            //return first line in an array of characters
            //ToCharArray converts string to array of chars
            char_arr = s[0].ToCharArray();
        }


        public char getNextChar()
        {
            linepos++; //linepos initialized to 0
                       //we didn't reach end of line
                       // && didn't reach end of file
            if (!(linepos < s[lineno].Length) && lineno < lines - 1) // we raeched end of line but not the end of file 
            {
                lineno++;
                linepos = 0;

                //end of line
                if (s[lineno].Length == 0)//replace the empty string between each two lines by a delimeter
                {
                    char_arr[linepos] = '\n';
                }
                //middle of line
                else // a non empty string
                    char_arr = s[lineno].ToCharArray();
                return char_arr[linepos];
            }
            //broke the first if because lineno = lines - 1
            //reached end of file
            else if (!(linepos < s[lineno].Length) && !(lineno < lines - 1)) //reached the end of line and there is no next line
            {
                lineno++;// increment lineno to break from the while loop in main
                linepos = 0;
                char_arr[linepos] = '\n';
            }
            return char_arr[linepos];

        }

    }

    public class MainClass
    {
        public static void Main()
        {
            FileReader fd = new FileReader();
            fd.readAllFile("C:\\Users\\ahmad\\Documents\\Visual Studio 2017\\Projects\\Compilers_Scanner\\Code.txt"); // replacable path

            Scanner Scanner = new Scanner(fd);
            Scanner.scanAndPrint();

            Console.ReadLine();
        }
    }

}
