﻿#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelatedRecords.Parser;
using com.calitha.goldparser;
using System.IO;
using System.Reflection;
using System.Configuration;
using static Common.Extensions;
using www.serviciipeweb.ro.iafblog.ExportDLL;
using Common;
using System.Data.SqlClient;
#endregion usings

namespace RelatedRecords.Data.ViewModels
{
    #region attributes

    public class CommandAttribute : Attribute
    {
        private SymbolConstants[] _symbols;
        public SymbolConstants[] Symbols
        {
            get { return _symbols; }
        }
        public CommandAttribute(params SymbolConstants[] symbols)
        {
            _symbols = symbols;
        }

        public override string ToString()
        {
            var value = string.Empty;
            _symbols.ToList().ForEach(s => value += "_" + ((int)s).ToString());

            return value;
        }
    }

    #endregion attributes

    public partial class MainViewModel
    {
        #region command handlers

        public void HandleCommand(ParseResults results)
        {
            var method = _commandMethods.First(m =>
                m.GetCustomAttributes(typeof(CommandAttribute), false)
                    .Cast<CommandAttribute>()
                    .First().ToString() == results.ToString());

            try
            {
                method.Invoke(this, new object[] { results.Tokens });
            } catch(Exception e)
            {
                ErrorLog.Error(e, "When running method {method} with {Tokens}", method, results.Tokens);
            }
        }

        [Command(SymbolConstants.SYMBOL_BACK)]
        public void Back(IEnumerable<TerminalToken> tokens)
        {
            DoBack();
        }

        [Command(SymbolConstants.SYMBOL_CLONE
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void CloneAsId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoCloneAsId(catalog);
        }

        [Command(SymbolConstants.SYMBOL_CLONE
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void CloneCatalogIdAsId(IEnumerable<TerminalToken> tokens)
        {
            var srcCatalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var tgtCatalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            DoCloneCatalogIdAsId(srcCatalog, tgtCatalog);
        }

        [Command(SymbolConstants.SYMBOL_CLONE
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void CloneCatalogId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoCloneCatalogId(catalog);
        }

        [Command(SymbolConstants.SYMBOL_CLONE)]
        public void Clone(IEnumerable<TerminalToken> tokens)
        {
            DoClone();
        }

        [Command(SymbolConstants.SYMBOL_COLUMNS
        , SymbolConstants.SYMBOL_INTEGER)]
        public void ColumnsInt(IEnumerable<TerminalToken> tokens)
        {
            var topN = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoColumnsInt(topN);
        }

        [Command(SymbolConstants.SYMBOL_COLUMNS)]
        public void Columns(IEnumerable<TerminalToken> tokens)
        {
            DoColumns();
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_HTML)]
        public void ExportAsHtml(IEnumerable<TerminalToken> tokens)
        {
            DoExportAsHtml();
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_JSON)]
        public void ExportAsJson(IEnumerable<TerminalToken> tokens)
        {
            DoExportAsJson();
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_SQL)]
        public void ExportAsSql(IEnumerable<TerminalToken> tokens)
        {
            DoExportAsSql();
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_HTML)]
        public void ExportIdAsHtml(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoExportIdAsHtml(table);
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_JSON)]
        public void ExportIdAsJson(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoExportIdAsJson(table);
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_SQL)]
        public void ExportIdAsSql(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoExportIdAsSql(table);
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_XML)]
        public void ExportIdAsXml(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoExportIdAsXml(table);
        }

        [Command(SymbolConstants.SYMBOL_EXPORT
        , SymbolConstants.SYMBOL_AS
        , SymbolConstants.SYMBOL_XML)]
        public void ExportAsXml(IEnumerable<TerminalToken> tokens)
        {
            DoExportAsXml();
        }

        [Command(SymbolConstants.SYMBOL_IMPORT
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_SERVER
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_USER
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_PASSWORD
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void ImportCatalogIdSvrIdUserIdPwdId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var server = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var user = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 2).Text;
            var password = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 3).Text;
            DoImportCatalogIdSvrIdUserIdPwdId(catalog, server, user, password);
        }

        [Command(SymbolConstants.SYMBOL_IMPORT
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_USER
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_PASSWORD
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void ImportCatalogIdUserIdPwdId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var user = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var password = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 2).Text;
            DoImportCatalogIdUserIdPwdId(catalog, user, password);
        }

        [Command(SymbolConstants.SYMBOL_IMPORT
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void ImportCatalogId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoImportCatalogId(catalog);
        }

        [Command(SymbolConstants.SYMBOL_LOAD
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void LoadCatalogId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoLoadCatalogId(catalog);
        }

        [Command(SymbolConstants.SYMBOL_LOAD)]
        public void Load(IEnumerable<TerminalToken> tokens)
        {
            DoLoad();
        }

        [Command(SymbolConstants.SYMBOL_RELATE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_TO
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_ON
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void RelateIdToIdOnIdEqId(IEnumerable<TerminalToken> tokens)
        {
            var srcTbl = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var tgtTbl = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var srcCol = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 2).Text;
            var tgtCol = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 3).Text;
            DoRelateIdToIdOnIdEqId(srcTbl, tgtTbl, srcCol, tgtCol);
        }

        [Command(SymbolConstants.SYMBOL_RELATE
        , SymbolConstants.SYMBOL_TO
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_ON
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void RelateToIdOnIdEqId(IEnumerable<TerminalToken> tokens)
        {
            var tgtTbl = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var srcCol = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var tgtCol = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 2).Text;
            DoRelateToIdOnIdEqId(tgtTbl, srcCol, tgtCol);
        }

        [Command(SymbolConstants.SYMBOL_REMOVE
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void RemoveCatalogId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoRemoveCatalogId(catalog);
        }

        [Command(SymbolConstants.SYMBOL_REMOVE)]
        public void Remove(IEnumerable<TerminalToken> tokens)
        {
            DoRemove();
        }

        [Command(SymbolConstants.SYMBOL_REFRESH)]
        public void Refresh(IEnumerable<TerminalToken> tokens)
        {
            DoRefresh();
        }

        [Command(SymbolConstants.SYMBOL_REFRESH
        , SymbolConstants.SYMBOL_CATALOG
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void RefreshCatalogId(IEnumerable<TerminalToken> tokens)
        {
            var catalog = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoRefreshCatalogId(catalog);
        }

        [Command(SymbolConstants.SYMBOL_ROOT)]
        public void Root(IEnumerable<TerminalToken> tokens)
        {
            DoRoot();
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_DEFAULT
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_STRINGLITERAL)]
        public void TableIdDefaultWhereIdEqStrLit(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_STRINGLITERAL, 0).Text;
            DoTableIdDefaultWhereIdEqStrLit(table, column, value);
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_DEFAULT)]
        public void TableIdDefault(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoTableIdDefault(table);
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_BETWEEN
        , SymbolConstants.SYMBOL_INTEGER
        , SymbolConstants.SYMBOL_AND
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdBetweenIntAndInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var minValue = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            var maxValue = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 1).Text;
            DoTableIdWhereIdBetweenValueAndValue(table, column, minValue, maxValue);
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_BETWEEN
        , SymbolConstants.SYMBOL_DECIMAL
        , SymbolConstants.SYMBOL_AND
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdBetweenDecAndDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var minValue = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            var maxValue = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 1).Text;
            DoTableIdWhereIdBetweenValueAndValue(table, column, minValue, maxValue, typeof(double));
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GTEQ
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdGtEqMinusInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value);
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GTEQ
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdGtEqMinusDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(double), ">=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GTEQ
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdGtEqInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(long), ">=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GTEQ
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdGtEqDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(double), ">=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GT
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdGtMinusInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(long), ">");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GT
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdGtMinusDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(double), ">");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GT
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdGtInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(long), ">");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_GT
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdGtDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(double), ">");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTEQ
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdLtEqMinusInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(long), "<=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTEQ
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdLtEqMinusDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(double), "<=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTEQ
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdLtEqInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(long), "<=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTEQ
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdLtEqDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(double), "<=");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTGT
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdLtGtInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(long), "<>");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTGT
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdLtGtDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(double), "<>");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTGT
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdLtGtMinusInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(long), "<>");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_LTGT
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdLtGtMinusDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(double), "<>");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdEqMinusInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(long));
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_MINUS
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdEqMinusDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, "-" + value, typeof(double));
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TableIdWhereIdEqInt(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(long));
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_EQ
        , SymbolConstants.SYMBOL_DECIMAL)]
        public void TableIdWhereIdEqDec(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_DECIMAL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(double));
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_IS
        , SymbolConstants.SYMBOL_NOT
        , SymbolConstants.SYMBOL_NULL)]
        public void TableIdWhereIdIsNotNull(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_NULL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(string), "is not");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_WHERE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_IS
        , SymbolConstants.SYMBOL_NULL)]
        public void TableIdWhereIdIsNull(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var column = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            var value = tokens.TerminalToken(SymbolConstants.SYMBOL_NULL, 0).Text;
            DoTableIdWhereIdOperatorValue(table, column, value, typeof(string), "is");
        }

        [Command(SymbolConstants.SYMBOL_TABLE
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void TableId(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoTableId(table);
        }

        [Command(SymbolConstants.SYMBOL_TABLES
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TablesInt(IEnumerable<TerminalToken> tokens)
        {
            var topN = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTablesInt(topN);
        }

        [Command(SymbolConstants.SYMBOL_TABLES)]
        public void Tables(IEnumerable<TerminalToken> tokens)
        {
            DoTables();
        }

        [Command(SymbolConstants.SYMBOL_TOP
        , SymbolConstants.SYMBOL_INTEGER)]
        public void TopInt(IEnumerable<TerminalToken> tokens)
        {
            var topN = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoTopInt(topN);
        }

        [Command(SymbolConstants.SYMBOL_UNRELATE
        , SymbolConstants.SYMBOL_IDENTIFIER
        , SymbolConstants.SYMBOL_TO
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void UnrelateIdToId(IEnumerable<TerminalToken> tokens)
        {
            var srcTbl = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            var tgtTbl = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 1).Text;
            DoUnrelateIdToId(srcTbl, tgtTbl);
        }

        [Command(SymbolConstants.SYMBOL_UNRELATE
        , SymbolConstants.SYMBOL_TO
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void UnrelateToId(IEnumerable<TerminalToken> tokens)
        {
            var tgtTbl = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoUnrelateToId(tgtTbl);
        }

        [Command(SymbolConstants.SYMBOL_CHILD)]
        public void Child(IEnumerable<TerminalToken> tokens)
        {
            DoChild();
        }

        [Command(SymbolConstants.SYMBOL_CHILD
        , SymbolConstants.SYMBOL_INTEGER)]
        public void ChildInt(IEnumerable<TerminalToken> tokens)
        {
            var index = tokens.TerminalToken(SymbolConstants.SYMBOL_INTEGER, 0).Text;
            DoChildInt(index);
        }

        [Command(SymbolConstants.SYMBOL_CHILD
        , SymbolConstants.SYMBOL_IDENTIFIER)]
        public void ChildId(IEnumerable<TerminalToken> tokens)
        {
            var table = tokens.TerminalToken(SymbolConstants.SYMBOL_IDENTIFIER, 0).Text;
            DoChildId(table);
        }

        [Command(SymbolConstants.SYMBOL_HELP)]
        public void Help(IEnumerable<TerminalToken> tokens)
        {
            DoHelp();
        }

        #endregion command handlers

        #region helper methods

        #region done methods

        private void DoBack()
        {
            if (_tableNavigation.Any())
            {
                CurrentTable = _tableNavigation.Pop();
            }
        }

        private void DoCloneAsId(string catalog)
        {
            DoCloneCatalogIdAsId(SelectedDataset.name, catalog);
        }

        private void DoCloneCatalogIdAsId(string srcCatalog, string tgtCatalog)
        {
            var srcDataset = findDataset(srcCatalog);
            if (null == srcDataset) return;

            var tgtDataset = findDataset(tgtCatalog);
            if (null != tgtDataset) return;

            tgtDataset = Extensions.Clone<CDataset>(srcDataset);
            var tgtDatasource = Extensions.Clone<CDatasource>(SelectedConfiguration
                .Datasource
                .First(x => x.name == srcDataset.dataSourceName));
            tgtDataset.name = tgtCatalog;
            tgtDataset.dataSourceName = tgtCatalog;
            tgtDatasource.name = tgtCatalog;
            SelectedConfiguration.Dataset.Add(tgtDataset);
            SelectedConfiguration.Datasource.Add(tgtDatasource);
            SaveConfiguration();
            LoadConfiguration();
        }

        private void DoCloneCatalogId(string catalog)
        {
            var srcDataset = findDataset(catalog);
            if (null == srcDataset) return;

            var tgtCatalog = catalog.RemoveLastNumbers();
            var count = SelectedConfiguration
                .Dataset
                .Where(x => x.name.ToLower().Contains(tgtCatalog.ToLower()))
                .Count();
            DoCloneCatalogIdAsId(catalog, tgtCatalog + count.ToString());
        }

        private void DoClone()
        {
            DoCloneCatalogId(SelectedDataset.name);
        }

        private void DoColumnsInt(string topN)
        {
            if (null != CurrentTable)
            {
                var table = findTable(CurrentTable.Root.ConfigTable.name);

                if (table != null)
                {
                    CurrentTable = _tableNavigation.Pop();
                }
                CurrentTable =
                    CurrentTable
                        .Root
                        .ConfigTable
                        .ToColumnsDataTable(int.Parse(topN))
                        .ToDatatableEx(CurrentTable.Root.ConfigTable);
                _tableNavigation.Push(CurrentTable);
            }
        }

        private void DoColumns()
        {
            DoColumnsInt("100");
        }

        #region export features

        private string ExportPath
        {
            get
            {
                string basePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "export");

                if (!Directory.Exists(basePath))
                {
                    Directory.CreateDirectory(basePath);
                }

                return basePath;
            }
        }

        private void DoExportAsHtml(bool includeChildren = true)
        {
            if(null != CurrentTable)
            {
                TraceLog.Information("Exporting tables to Html");

                string templatePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    @"templates\Collection\html_tables.st");
                string basePath = ExportPath;

                string namePart = DateTime.Now.ToString("yyyy-MMM-dd.hh-mm-ss");
                StringBuilder html = new StringBuilder();
                html.Append(File.ReadAllText(templatePath).Replace("$FirstItem.Name;format=\"toUpper\"$", namePart)
                    .Replace("$DateCreated;format=\"yyyy MMM dd\"$", DateTime.Now.ToString("MMM-dd-yyyy hh:mm:ss")));

                StringBuilder tables = new StringBuilder();
                int level = 0;
                fillHtml(CurrentTable, ref tables, ref level, includeChildren);

                string htmlFile = Path.Combine(basePath, string.Format("{0}.html", namePart));
                File.WriteAllText(htmlFile, html.ToString().Replace("<$Tables>", tables.ToString()));

                runProcess(ConfigurationManager.AppSettings["fileExplorer"], htmlFile, -1);
            }
        }

        private void fillHtml(DatatableEx t, ref StringBuilder tables, ref int level, bool includeChildren)
        {
            string[] levels = new string[] {
                "first", "second", "third", "fourth", "fifth", "sixth", "seventh", "eighth", "nineth", "tenth"
            };
            tables.Append(t.Root.Table.ExportAsHtmlFragment()
                .Replace("className", string.Format("{0}LevelTable", levels[level]))
                .Replace("tableName", t.Root.Table.TableName.ToUpper())
                .Replace("columnCount", t.Root.Table.Columns.Count.ToString()));

            if (includeChildren)
            {
                ++level;
                foreach (var child in t.Children)
                    fillHtml(child, ref tables, ref level, includeChildren);
                --level;
            }
        }

        private void DoExportAsJson(bool includeChildren = true)
        {
        }

        private void DoExportAsSql(bool includeChildren = true)
        {
            TraceLog.Information("Exporting tables to SQL Insert");

            string sqlFile = Path.Combine(ExportPath, DateTime.Now.ToString("yyyy-MMM-dd.hh-mm-ss") + ".sql");
            using (var stream = new StreamWriter(sqlFile))
            {
                stream.Write(new StringBuilder().SqlInsert(CurrentTable, includeChildren).ToString());
            }

            runProcess(ConfigurationManager.AppSettings["fileExplorer"], sqlFile, -1);
        }

        private void DoExportIdAsHtml(string tableName, bool includeChildren = true)
        {
            var table = findTable(tableName);
            if (null == table) ThrowError("Invalid table {0}", tableName);

            DoTableId(tableName);
            DoExportAsHtml(includeChildren);
        }

        private void DoExportIdAsJson(string tableName, bool includeChildren = true)
        {
        }

        private void DoExportIdAsSql(string tableName, bool includeChildren = true)
        {
            var table = findTable(tableName);
            if (null == table) ThrowError("Invalid table {0}", tableName);

            DoTableId(tableName);
            DoExportAsSql(includeChildren);
        }

        private void DoExportIdAsXml(string tableName, bool includeChildren = true)
        {
        }

        private void DoExportAsXml(bool includeChildren = true)
        {
        }

        #endregion export features

        private async void DoImportCatalogIdSvrIdUserIdPwdId(string catalog, string server, string user = "", string password = "")
        {
            var connStr = string.Format("data source={0};initial catalog={1};{2}MultipleActiveResultSets=True;asynchronous processing=True;Enlist=false;",
                server, catalog,
                !string.IsNullOrWhiteSpace(user)
                    ? string.Format("User Id={1};Password={1};", user, password)
                    : "Integrated Security=True;");
            var newCfg = XmlHelper<CConfiguration>.Load(
                await Helpers.GetConfigurationFromConnectionString(connStr)
            );

            if (null == newCfg || !newCfg.Dataset.Any()) return;

            newCfg.Dataset.FirstOrDefault().defaultTable = newCfg.Dataset.First().Table.First().name;
            var sourceName = newCfg.Datasource.First().name.RemoveLastNumbers();
            var namesFound = SelectedConfiguration.Datasource.Where(s => s.name.Contains(sourceName)).Count();
            if (namesFound > 0)
            {
                sourceName += namesFound.ToString();
                newCfg.Datasource.First().name = sourceName;
                newCfg.Dataset.First().dataSourceName = sourceName;
                newCfg.Dataset.First().name = sourceName;
            }
            SelectedConfiguration.Datasource.Add(newCfg.Datasource.First());
            SelectedConfiguration.Dataset.Add(newCfg.Dataset.First());

            SaveConfiguration();
            LoadConfiguration();
        }

        private void DoImportCatalogIdUserIdPwdId(string catalog, string user, string password)
        {
            DoImportCatalogIdSvrIdUserIdPwdId(catalog, 
                ConfigurationManager.AppSettings["localdb"].ToString(), 
                user, 
                password);
        }

        private void DoImportCatalogId(string catalog)
        {
            DoImportCatalogIdSvrIdUserIdPwdId(catalog,
                ConfigurationManager.AppSettings["localdb"].ToString());
        }

        private void DoLoadCatalogId(string catalog)
        {
            var ds = findDataset(catalog);
            if (ds == null) ThrowError("Invalid catalog {0}", catalog);

            SelectedDataset = ds;
            DoLoad();
        }

        private async void DoLoad()
        {
            var isCurrent = TableIsCurrent(SelectedDataset.defaultTable);
            CurrentTable = await findTable(SelectedDataset.defaultTable)
                .Query("".ToArray(), "".ToArray(), true);

            if(isCurrent)
            {
                _tableNavigation.Pop();
            }
            _tableNavigation.Push(CurrentTable);
        }

        private void DoRelateIdToIdOnIdEqId(string srcTblName, string tgtTblName, string srcColName, string tgtColName)
        {
            var srcTable = SelectedDataset
                .Table
                .FirstOrDefault(x =>
                    x.name.ToLower() == srcTblName.ToLower());

            if (null == srcTable) return;

            var tgtTable = SelectedDataset
            .Table
            .FirstOrDefault(x =>
                x.name.ToLower() == tgtTblName.ToLower());

            if (null == tgtTable) return;

            var srcCol = srcTable
                .Column
                .FirstOrDefault(x => x.name.ToLower() == srcColName.ToLower());

            if (null == srcCol) return;

            var tgtCol = tgtTable
                .Column
                .FirstOrDefault(x => x.name.ToLower() == tgtColName.ToLower());

            if (null == tgtCol || srcCol.DbType != tgtCol.DbType) return;

            var relationship = new CRelationship
            {
                name = string.Format("{0}->{1}", srcTable.name, tgtTable.name),
                fromTable = srcTable.name,
                toTable = tgtTable.name,
                fromColumn = srcCol.name,
                toColumn = tgtCol.name
            };

            if (SelectedDataset.Relationship.Any(x => x.name == relationship.name)) return;

            var currentDatasetName = SelectedDataset.name;
            var currentTableName = null != CurrentTable ? CurrentTable.Root.ConfigTable.name : string.Empty;

            SelectedDataset.Relationship.Add(relationship);
            SaveConfiguration();
            LoadConfiguration();
        }

        private void DoRelateToIdOnIdEqId(string tgtTblName, string srcColName, string tgtColName)
        {
            var currentTableName = null != CurrentTable
                ? CurrentTable.Root.ConfigTable.name
                : SelectedDataset.defaultTable;

            DoRelateIdToIdOnIdEqId(currentTableName, tgtTblName, srcColName, tgtColName);
        }

        private void DoRemoveCatalogId(string datasetName)
        {
            if (SelectedConfiguration.Dataset.Count > 1)
            {
                var currentDatasetName = SelectedDataset.name;
                var dataSet = SelectedConfiguration
                    .Dataset
                    .FirstOrDefault(x =>
                    x.name.ToLower() == datasetName.ToLower());
                if (null == dataSet) return;

                SelectedConfiguration.Dataset.Remove(dataSet);
                if (SelectedConfiguration.defaultDataset == currentDatasetName)
                {
                    SelectedConfiguration.defaultDataset = SelectedConfiguration.Dataset.First().name;
                    SelectedConfiguration.defaultDatasource = SelectedConfiguration.Dataset.First().dataSourceName;
                }

                SaveConfiguration();
                LoadConfiguration();
            }
        }

        private void DoRemove()
        {
            DoRemoveCatalogId(SelectedDataset.name);
        }

        private void DoRefresh()
        {
            DoRefreshCatalogId(SelectedDataset.name);
        }

        private async void DoRefreshCatalogId(string catalog)
        {
            var dataset = findDataset(catalog);
            if (null == dataset) return;

            var config = XmlHelper<CConfiguration>.Load(
                await Helpers.GetConfigurationFromConnectionString(
                    SelectedConfiguration.Datasource.First(ds => ds.name == dataset.dataSourceName)
                        .ConnectionString)
                );

            if (null == config || !config.Dataset.Any()) return;
            
            dataset.Table.Clear();
            config.Dataset.First().Table.ToList().ForEach(t =>
                dataset.Table.Add(t)
            );

            SaveConfiguration();
            LoadConfiguration();
        }

        private void DoRoot()
        {
            _tableNavigation.Clear();
            DoLoad();
        }

        private void DoTableIdDefaultWhereIdEqStrLit(string tableName, string column, string value)
        {
            var table = findTable(tableName);

            if (null == table) return;

            if (SelectedDataset.defaultTable != table.name)
            {
                SelectedDataset.defaultTable = table.name;
                SaveConfiguration();
            }
            DoTableIdWhereIdOperatorValue(tableName, column, value.Replace("\"", string.Empty), typeof(string), "=");
        }

        private void DoTableIdDefault(string tableName)
        {
            var table = findTable(tableName);

            if (null == table) return;

            if (SelectedDataset.defaultTable != table.name)
            {
                SelectedDataset.defaultTable = table.name;
                SaveConfiguration();
            }
            DoTableId(tableName);
        }

        #endregion done methods

        private async void DoTableIdWhereIdBetweenValueAndValue(string tableName, string columnName, 
            string minValue, string maxValue, Type type = null)
        {
            var table = findTable(tableName);
            if (null == table || !table.Column.Any(x => x.name.ToLower() == columnName.ToLower()))
                ThrowError("Invalid table {0}, column: {1}", tableName, columnName);

            var isCurrent = TableIsCurrent(tableName);

            CurrentTable = await table.Query(">=,<=".ToArray(),
                        "And".ToArray(),
                        false,
                        new SqlParameter(columnName, ParseValue(minValue, type)),
                        new SqlParameter(columnName, ParseValue(maxValue, type)));

            if(isCurrent)
            {
                _tableNavigation.Pop();
            }
            _tableNavigation.Push(CurrentTable);
        }

        private async void DoTableIdWhereIdOperatorValue(string tableName, string columnName, 
            string value, Type type = null, string compOperator = "")
        {
            var table = findTable(tableName);
            if (null == table || !table.Column.Any(x => x.name.ToLower() == columnName.ToLower()))
                ThrowError("Invalid table {0}, column: {1}", tableName, columnName);

            var isCurrent = TableIsCurrent(tableName);

            CurrentTable = await table.Query(compOperator.ToArray(),
                        "".ToArray(),
                        false,
                        new SqlParameter(columnName, ParseValue(value, type)));

            if (isCurrent)
            {
                _tableNavigation.Pop();
            }
            _tableNavigation.Push(CurrentTable);
        }

        private async void DoTableId(string tableName, int topN = 1000)
        {
            var table = findTable(tableName);
            if (null == table) ThrowError("Invalid table {0}", tableName);

            var isCurrent = TableIsCurrent(tableName);
            Extensions.MaxRowCount = topN;
            CurrentTable = await table.Query("".ToArray(), "".ToArray(), true);

            if (isCurrent)
            {
                _tableNavigation.Pop();
            }

            _tableNavigation.Push(CurrentTable);
        }

        private void DoTopInt(string topN)
        {
            DoTableId(SelectedDataset.defaultTable, int.Parse(topN));
        }

        private void DoTablesInt(string topN)
        {
            DoTables(int.Parse(topN));
        }

        private void DoTables(int topN = 1000)
        {
            var isCurrent = TableIsCurrent(SelectedDataset.name);

            CurrentTable = SelectedDataset
                .ToDataTable(topN)
                .ToDatatableEx(SelectedDataset.ToTable());

            if(isCurrent)
            {
                _tableNavigation.Pop();
            }
            _tableNavigation.Push(CurrentTable);
        }

        private void DoUnrelateIdToId(string srcTblName, string tgtTblName)
        {
            var srcTable = SelectedDataset
                .Table
                .FirstOrDefault(x =>
                    x.name.ToLower() == srcTblName.ToLower());

            if (null == srcTable) return;

            var tgtTable = SelectedDataset
            .Table
            .FirstOrDefault(x =>
                x.name.ToLower() == tgtTblName.ToLower());

            if (null == tgtTable || srcTable.name == tgtTable.name) return;

            var relationship = SelectedDataset
                .Relationship.FirstOrDefault(x =>
                    x.name == CRelationship.GetName(srcTable.name, tgtTable.name));
            if (null == relationship) return;

            SelectedDataset.Relationship.Remove(relationship);
            SaveConfiguration();
            LoadConfiguration();
        }

        private void DoUnrelateToId(string tgtTblName)
        {
            var currentTableName = null != CurrentTable
                ? CurrentTable.Root.ConfigTable.name
                : SelectedDataset.defaultTable;
            DoUnrelateIdToId(currentTableName, tgtTblName);
        }

        private void DoChild()
        {
            DoChildInt("0");
        }

        private void DoChildInt(string index)
        {
            var table = findDataTableByIndex(int.Parse(index));
            if (null == table) ThrowError("Invalid table at Index {0}", index);

            _tableNavigation.Push(CurrentTable);
            CurrentTable = table;
        }

        private void DoChildId(string tableName)
        {
            var table = findDataTable(tableName);
            if (null == table) ThrowError("Invalid table {0}", tableName);

            _tableNavigation.Push(CurrentTable);
            CurrentTable = table;
        }

        private void DoHelp()
        {
            #region commands
            var commands = @"
back
child [TableName | index]
clone
clone as NewCatalogName
clone catalog CatalogName as NewCatalogName
clone catalog CatalogName
columns [topN]
export [TableName] as html | sql | json | xml
help
import catalog NewCatalogName
import catalog NewCatalogName user UserName password Password
import catalog NewCatalogName server ServerName user UserName password Password
load [catalog CatalogName]
refresh [catalog CatalogName]
relate [SourceTableName] to TargetTableName on SourceColumn = TargetColumn
remove [catalog CatalogName]
root
table TableName [default] [where Column Operator (Value | MinValue and MaxValue)]
tables [topN]
top topN
unrelate [SourceTableName] to TargetTableName";

            var descriptions = @"
Navigates back (to parent/previous table)
Drill down into children using name or index position
Clones current catalog
Clones current catalog as NewCatalogName
Clones CatalogName as NewCatalogName
Clones CatalogName as Default new catalog name
Displays current table top N columns
Exports current or specified table as html, sql, json or xml
Displays this help
Imports an existing Database catalog 
Imports an existing Database catalog providing user and password values
Imports an existing Database catalog providing server name, user and password values
Loads current or specified catalog name and sets as default
Refreshes current or specified catalog schema definition
Relates current or specified table to another table using parent/child relationship on its columns
Removes current or specified catalog name
Navigate to root (default dataset table)
Queries a table and optionally sets as default dataset table. Operator can be any standard T-Sql operator and Value is any standard T-Sql value
Display top n dataset tables (its names and columns count)
Displays top n records of current table
Unrelates current or specified table to child table";
            #endregion commands

            var table = new string[] { commands, descriptions }
                .ToDatatableEx();

            if(TableIsCurrent("help"))
            {
                _tableNavigation.Pop();
            }

            CurrentTable = table;
            _tableNavigation.Push(CurrentTable);
        }

        #endregion helper methods

        #region utility methods 

        private DatatableEx findDataTable(string name)
        {
            if (null == CurrentTable) return null;

            return CurrentTable
                .Children
                .FirstOrDefault(x => x.Root.ConfigTable.name.ToLower() == name.ToLower());
        }

        private DatatableEx findDataTableByIndex(int index)
        {
            if (null == CurrentTable 
                || index < 0 
                || index > CurrentTable.Children.Count-1) return null;

            return CurrentTable
                .Children
                .ElementAt(index);
        }

        private CTable findTable(string name)
        {
            return SelectedDataset
                .Table
                .FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }

        private CDataset findDataset(string name)
        {
            return SelectedConfiguration
                .Dataset
                .FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }

        private object ParseValue(string value, Type type)
        {
            var valType = type ?? typeof(long);
            switch (valType.ToString())
            {
                case "System.Int64":
                    return Int64.Parse(value);
                case "System.Double":
                    return Double.Parse(value);
                default:
                    return value;
            }
        }

        private bool TableIsCurrent(string tableName)
        {
            return (null != CurrentTable
                && CurrentTable.Root.ConfigTable.name.ToLower() == tableName.ToLower());
        }

        private void ThrowError(string format, params string[] args)
        {
            //throw new Exception(string.Format(format, args));
            ErrorLog.Error(format, args);
        }

        #endregion utility methods

        #region utility classes

        internal class State
        {
            private string _datasetName;
            private string _tableName;
            private List<DatatableEx> _navigation;

            private readonly MainViewModel _model;
            public State(MainViewModel model)
            {
                _model = model;
            }

            public bool SaveState()
            {
                if (null != _model.SelectedDataset)
                {
                    _datasetName = _model.SelectedDataset.name;
                    _tableName = null != _model.CurrentTable
                        ? _model.CurrentTable.Root.ConfigTable.name
                        : _model.SelectedDataset.defaultTable;
                    _navigation = _model.TableNavigation.ToList();
                    return true;
                }

                return false;
            }

            public async void RestoreState()
            {
                var ds = _model
                    .SelectedConfiguration
                    .Dataset
                    .FirstOrDefault(x => x.name == _datasetName);
                if (null == ds)
                {
                    ds = _model.SelectedConfiguration.Dataset.First();
                }
                _model.SelectedDataset = ds;

                foreach (var t in _navigation)
                {
                    _model.TableNavigation.Push(
                        await t.Root.ConfigTable.Query("".ToArray(), "".ToArray(), true));
                }

                _model.CurrentTable = _navigation.FirstOrDefault(x => x.Root.ConfigTable.name == _tableName);
            }
        }

        #endregion
    }
}
