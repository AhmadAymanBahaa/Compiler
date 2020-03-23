using System;
using System.Collections.Generic;
using System.IO;

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


public class Compiler
{

	public static void Main()
	{
		FileReader fd = new FileReader();
		fd.readAllFile("C:\\Users\\Mariam Fayed\\Downloads\\instructions.txt"); // replacable path

		while (true)
		{
			//if reached end of file, break.

			if (!(fd.lineno < fd.lines))
				break;
			Console.Write(fd.getNextChar()); // print next character
		}

	}



}
