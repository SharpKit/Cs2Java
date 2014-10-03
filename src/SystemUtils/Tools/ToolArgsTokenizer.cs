using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SystemUtils.Tools
{
    public class ToolArgsTokenizer
	{

		bool InString;
		StringBuilder CurrentToken;
		List<string> Tokens;

		StringBuilder Processed = new StringBuilder();
		public string[] Tokenize(string text)
		{
			Tokens = new List<string>();
			CurrentToken = new StringBuilder();
			InString = false;
			foreach (char ch in text)
			{
				if (ch == '\"')
				{
					if (InString)
					{
						InString = false;
						ContinueToken(ch);
					}
					else
					{
						InString = true;
						BeginOrContinueToken(ch);
					}
				}
				else if (ch == ' ')
				{
					if (InString)
						ContinueToken(ch);
					else
						EndTokenIfExists();
				}
				else
				{
					BeginOrContinueToken(ch);
				}
				Processed.Append(ch);
			}
			EndTokenIfExists();
			return Tokens.ToArray();
		}



		private void BeginToken(char ch)
		{
			BeginToken();
			CurrentToken.Append(ch);
		}
		private void BeginToken()
		{
			if (CurrentToken != null)
				throw new Exception("cannot begin word in the middle of another word");
			CurrentToken = new StringBuilder();
		}
		private void BeginOrContinueToken(char ch)
		{
			if (CurrentToken == null)
				CurrentToken = new StringBuilder();
			CurrentToken.Append(ch);
		}
		private void ContinueToken(char ch)
		{
			CurrentToken.Append(ch);
		}
		void EndToken()
		{
			var s = CurrentToken.ToString();
			if(s.IsNotNullOrEmpty())
				Tokens.Add(s);
			CurrentToken = null;
		}
		private void EndTokenIfExists()
		{
			if (CurrentToken != null)
				Tokens.Add(CurrentToken.ToString());
			CurrentToken = null;
		}

	}
}
