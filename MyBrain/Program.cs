using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace MyBrain
{
	class Program
	{
		public static Dictionary< string, string > InputWordDictionary { get; set; } = new Dictionary< string, string >();

		public static Dictionary< string, string > InputPhraseDictionary { get; set; } = new Dictionary< string, string >();

		static void Main()
		{
			//string input = GetInput();

			while ( GetInput() != "=" ) { }
			
			Console.WriteLine( "Press any key to end" );
			Console.ReadKey();
		}

		
		public static string GetInput()
		{
			StopWatch.Start();

			bool reading = true;
			Console.WriteLine( "Type input:" );

			string inputPhrase = "";

			string inputWord = "";

			while ( reading )
			{
				ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

				if ( !string.IsNullOrWhiteSpace( consoleKeyInfo.KeyChar.ToString() ) )
					inputWord += consoleKeyInfo.KeyChar;

				if ( consoleKeyInfo.Key == ConsoleKey.Spacebar || consoleKeyInfo.Key == ConsoleKey.Enter )
				{
					if ( !InputWordDictionary.ContainsKey( inputWord ) )
					{
						var word = inputWord;
						Task.Factory.StartNew( () =>
						{
							string definition = word.GetDefinition();
							lock ( InputWordDictionary )
							{
								InputWordDictionary.Add( word, definition );
							}
							foreach ( string s in definition.Split( ' ' ) )
							{
								if( !InputWordDictionary.ContainsKey( s ))
									lock ( InputWordDictionary )
									{
										InputWordDictionary.Add( s, s.GetDefinition() );
									}
							}
						} );
						if ( Debugger.IsAttached ) Debug.WriteLine( inputWord );
					}

					inputPhrase = string.IsNullOrEmpty( inputPhrase ) ? $"{inputWord}" : $"{inputPhrase} {inputWord}";
					inputWord = "";
				}

				reading = consoleKeyInfo.Key != ConsoleKey.Enter;
			}

			if ( !InputPhraseDictionary.ContainsKey( inputPhrase ) )
				InputPhraseDictionary.Add( inputPhrase, "" );
			return inputPhrase;
		}
	}

	public static class Extensions
	{
		public static string GetDefinition( this string word )
		{
			StopWatch.Elapsed( $"Defining '{word}'", false );
			string definition = "";

			Uri uri = new Uri( $@"https://en.wikipedia.org/wiki/{word}" );
			
			HtmlDocument htmlDocument = new HtmlDocument();
			htmlDocument.LoadHtml( uri.GetHtmlStream() );

			HtmlNodeCollection htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes( $"//p" );
			if ( htmlNodeCollection == null ) return definition;
			foreach ( HtmlNode htmlNode in htmlNodeCollection )
			{
				definition += htmlNode.InnerText;
				if ( definition.EndsWith( "may refer to:" ) )
				{
					HtmlNodeCollection listHtmlNodeCollection = htmlDocument.DocumentNode.SelectNodes( $"//li" );
					if ( listHtmlNodeCollection != null )
					{
						foreach ( HtmlNode listHtmlNode in listHtmlNodeCollection )
						{
							definition += Environment.NewLine + listHtmlNode.InnerText;
						}
					}
				}
				break;
			}
			return definition;
		}

		public static string GetHtmlStream( this Uri uri )
		{
			string content = "";
			try
			{
				WebRequest request = WebRequest.Create( uri );
				WebResponse response = request.GetResponse();
				Stream stream = response.GetResponseStream();
				StreamReader reader = new StreamReader( stream );
				content = reader.ReadToEnd();
				reader.Close();
				response.Close();

			}
			catch ( Exception e )
			{
				if ( Debugger.IsAttached )
					Debug.WriteLine( e );
			}
			return content;
		}
	}
}
