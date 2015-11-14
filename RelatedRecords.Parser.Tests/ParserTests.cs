﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;
using static Common.Extensions;

namespace RelatedRecords.Parser.Tests
{
    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void RelatedRecords_Parser_Tests()
        {
            #region text commands

            var commands = @"
back
clone as SomeOtherName
clone catalog MyCatalog as SomeOther12_catName
clone catalog SampleCat
clone
columns 2
columns
export as html
export as json
export as sql
export Html_Table as html
export _Tablename as json
export _Table_Name_12 as sql
export ThisTable as xml
export as xml
import catalog SampleZ server localhostz user devz password pwdz
import catalog SampleY user devy password pwdy
import catalog SampleX
load catalog CatalogName
load
relate ThisTable to OtherTable12 on Column1 = column_2
relate to OtherTable12 on Column1 = column_2
remove catalog CatalogName
remove
root
table Test21 default where col1 = '1.34'
table Test21 default
table Test21 where col1 between 1.34 and 245.234
table Test21 where col1 >= 121.340
table Test21 where col_some12 is not null
table Test21 where col12xo is null
table Test21
tables 10
tables
top 100
unrelate This_Table12 to OtherTable12
unrelate to OtherTable12
";
            #endregion text commands

            var parser = new RRParser(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "relatedrecords.cgt"));

            using (var stream = File.CreateText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "methods.cs")))
            {
                var methods = new StringBuilder();

                foreach (var cmd in commands.Replace("'", "\"").Split(Environment.NewLine.ToCharArray(),
                    StringSplitOptions.RemoveEmptyEntries))
                {
                    var results = parser.Parse(cmd);
                    Assert.IsTrue(results.isAccepted);

                    methods.Append("[Command(");
                    foreach (var t in results.Tokens)
                    {
                        methods.AppendFormat("{0}SymbolConstants.{1}{2}",
                            t == results.Tokens.First() ? string.Empty : ",",
                            t.Symbol.SymbolEnum(),
                            t == results.Tokens.Last() ? string.Empty : Environment.NewLine);
                    }
                    methods.AppendFormat(")]{0}", Environment.NewLine);

                    methods.Append("public void ");
                    foreach(var t in results.Tokens)
                    {
                        methods.Append(CapitalizeWords(t.Symbol.Name));
                    }
                    methods.AppendFormat("(IEnumerable<TerminalToken> tokens){0}",
                        Environment.NewLine);

                    methods.AppendFormat("{{{0}", Environment.NewLine);
                    methods.AppendFormat("}}{0}{0}", Environment.NewLine);
                }

                stream.Write(methods.ToString());
            }
        }
    }
}
