using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Common;
using System.Collections;
using System.Configuration;
using Axon.DAL; 

/// <summary>
/// Descripcion :
/// Autor       :
/// Fecha       :
/// Modificacion:
/// </summary>
public class DBAxon
{

   

    #region DECLARATIONS
    Axon.DAL.Conexion oConexion = new Axon.DAL.Conexion();
    private DbProviderFactory factory;
    public DbConnection connection;
    private ConnectionState connectionState;
    public DbCommand command;
    private DbParameter parameter;
    public DbTransaction transaction;
    private bool mblTransaction;
    public int startRecord = 0;
    public int maxRecord = 0; //Todos los registros pro defecto
    public int timeOut = 0; //Por defecto x siempre
      
    //Informix : Database=tbsfi;Host=192.168.100.10;Server=db_engine_tcp;Service=1525; Protocol=onsoctcp;UID=myUsername;Password=myPassword;

    //private static readonly string S_CONNECTION = @"Provider=SQLOLEDB;Data Source=FPEREYRA\sqlexpress;Initial Catalog=espia50;User Id=fpereyra;Password=123"; //ConfigurationManager.AppSettings["DATA.CONNECTIONSTRING"];
    //private static readonly string S_PROVIDER = "System.Data.OleDb"; //ConfigurationManager.AppSettings["DATA.PROVIDER"];

    //private static readonly string S_CONNECTION = @"Data Source=FPEREYRA\sqlexpress;Initial Catalog=espia50;User ID=fpereyra;Password=123"; //ConfigurationManager.AppSettings["DATA.CONNECTIONSTRING"];
    //private static readonly string S_PROVIDER = "System.Data.SqlClient"; //ConfigurationManager.AppSettings["DATA.PROVIDER"];

    private const char TOKEN_BD = '@';


    #endregion

    #region ENUMERATORS

    //public enum TransactionType : uint
    //{
    //    BeginTransaction = 1,
    //    Commit = 2,
    //    Rollback = 3
    //}

    #endregion

    #region STRUCTURES

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp 
    ///Date				:	15/02/2009
    ///Input			:	
    ///OutPut			:	
    ///Comments			:	
    /// </summary>
    public struct Parameters
    {
        public string paramName;
        public object paramValue;
        public ParameterDirection paramDirection;
        public DbType paramType;
        public int paramSize;
        public int paramScale;

        public Parameters(string Name, object Value, ParameterDirection direction, DbType type, int size)
            : this()
        {
            paramName = Name;
            paramValue = Value;
            paramType = type;
            paramSize = size;
            paramDirection = direction;
        }

        public Parameters(string name, object value, ParameterDirection direction, DbType type)
            : this()
        {
            paramName = name;
            paramValue = value;
            paramType = type;
            paramDirection = direction;
        }

        public Parameters(string name, object value, ParameterDirection direction) :this()
        {
            paramName = name;
            paramValue = value;
            paramDirection = direction;
        }

        public Parameters(string name, object value) : this()
        {
            
            paramName = name;
            paramValue = value;
            paramDirection = ParameterDirection.Input;
        }


    }

    #endregion

    #region CONSTRUCTOR

    public DBAxon()
    {
        factory = DbProviderFactories.GetFactory(Conexion.provider);
    }

    #endregion

    #region DESTRUCTOR

    ~DBAxon()
    {
        factory = null;
    }

    #endregion

    #region CONNECTIONS

    /// <summary>
    ///Description	    :	This function is used to Open Database Connection
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	NA
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    public void OpenFactoryConnection()
    {
        /*
        // This check is not required as it will throw "Invalid Provider Exception" on the contructor itself.
        if (0 == DbProviderFactories.GetFactoryClasses().Select("InvariantName='" + S_PROVIDER + "'").Length)
            throw new Exception("Invalid Provider");
        */
        connection = factory.CreateConnection();

        if (connection.State == ConnectionState.Closed)
        {
            connection.ConnectionString = Conexion.strConn;
            connection.Open();
            connectionState = ConnectionState.Open;
        }
    }

    /// <summary>
    ///Description	    :	This function is used to Close Database Connection
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	NA
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    public void CloseFactoryConnection()
    {
        //check for an open connection            
        try
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                connectionState = ConnectionState.Closed;
            }
        }
        catch (DbException oDbErr)
        {
            //catch any SQL server data provider generated error messag
            throw new Exception(oDbErr.Message);
        }
        catch (System.NullReferenceException oNullErr)
        {
            throw new Exception(oNullErr.Message);
        }
        finally
        {
            if (null != connection)
                connection.Dispose();
        }
    }

    #endregion

    #region TRANSACTION

    /// <summary>
    ///Description	    :	This function is used to Handle Transaction Events
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Transaction Event Type
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    /// 
    public void BeginTransaction()
    {
        transaction = connection.BeginTransaction();
        mblTransaction = true;
    }

    public void BeginTransaction(IsolationLevel veIsolationLevel)
    {
        transaction = connection.BeginTransaction(veIsolationLevel);
        mblTransaction = true;
    }

    public void CommitTransaction()
    {
        transaction.Commit();
        mblTransaction = false;
    }

    public void RollbackTransaction()
    {
        if (mblTransaction)
        {
            transaction.Rollback();
            mblTransaction = false;
        }
    }

    //public void TransactionHandler(TransactionType veTransactionType)
    //{
    //    switch (veTransactionType)
    //    {
    //        case TransactionType.BeginTransaction:  //open a transaction
    //            try
    //            {
    //                transaction = connection.BeginTransaction(IsolationLevel);
    //                mblTransaction = true;
    //            }
    //            catch (InvalidOperationException oErr)
    //            {
    //                throw new Exception("@TransactionHandler - " + oErr.Message);
    //            }
    //            break;

    //        case TransactionType.Commit:  //commit the transaction
    //            if (null != transaction.Connection)
    //            {
    //                try
    //                {
    //                    transaction.Commit();
    //                    mblTransaction = false;
    //                }
    //                catch (InvalidOperationException oErr)
    //                {
    //                    throw new Exception("@TransactionHandler - " + oErr.Message);
    //                }
    //            }
    //            break;

    //        case TransactionType.Rollback:  //rollback the transaction
    //            try
    //            {
    //                if (mblTransaction)
    //                {
    //                    transaction.Rollback();
    //                }
    //                mblTransaction = false;
    //            }
    //            catch (InvalidOperationException oErr)
    //            {
    //                throw new Exception("@TransactionHandler - " + oErr.Message);
    //            }
    //            break;
    //    }

    //}

    #endregion

    #region COMMANDS

    #region PARAMETERLESS METHODS


    public DbProviderFactory Factory()
    {
        return factory;
    }
    /// <summary>
    /// Descripcion    : Crea un parametro desde la factoria de parametros
    /// Author         : fpp
    /// </summary>
    /// <returns>DbParameter</returns>

    public DbParameter CreateParameter(string Name, object Value, ParameterDirection direction, DbType type, int size)
    {
        DbParameter p1 = factory.CreateParameter();
        p1.ParameterName = Name;
        p1.DbType = type;
        p1.Direction = direction;
        p1.Size = size;
        p1.Value = Value;
        return p1;
    }

    public DbParameter CreateParameter(string Name, object Value, ParameterDirection direction, DbType type)
    {
        DbParameter p1 = factory.CreateParameter();
        p1.ParameterName = Name;
        p1.DbType = type;
        p1.Direction = direction;
        p1.Value = Value;
        return p1;
    }

    


    /// <summary>
    ///Descripcion	    :	This function is used to Prepare Command For Execution
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	NA
    ///Comments			:	Has to be changed/removed if object based array concept is removed.
    /// </summary>
    private void PrepareCommand(CommandType cmdType, string cmdText)
    {

        if (connection.State != ConnectionState.Open)
        {
            connection.ConnectionString = Conexion.strConn;
            connection.Open();
            connectionState = ConnectionState.Open;
        }

        if (null == command)
            command = factory.CreateCommand();

        command.Connection = connection;
        command.CommandText = cmdText;
        command.CommandType = cmdType;
        command.CommandTimeout = timeOut;  
        if (mblTransaction) command.Transaction = transaction;
    }

    #endregion

    #region OBJECT BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to Prepare Command For Execution
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    private void PrepareCommand(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms)
    {

        if (connection.State != ConnectionState.Open)
        {
            connection.ConnectionString = Conexion.strConn;
            connection.Open();
            connectionState = ConnectionState.Open;
        }

        if (null == command)
            command = factory.CreateCommand();

        command.Connection = connection;
        command.CommandText = cmdText;
        command.CommandType = cmdType;
        command.CommandTimeout = timeOut;  

        if (blTransaction)
            command.Transaction = transaction;

        if (null != cmdParms)
            CreateDBParameters(cmdParms);
    }

    #endregion

    #region STRUCTURE BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to Prepare Command For Execution
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    public void PrepareCommand(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
    {

        if (connection.State != ConnectionState.Open)
        {
            connection.ConnectionString = Conexion.strConn;
            connection.Open();
            connectionState = ConnectionState.Open;
        }

        command = factory.CreateCommand();
        command.Connection = connection;
        command.CommandText = ConvertCmdTextToBD(cmdText);
        command.CommandType = cmdType;
        command.CommandTimeout = timeOut;  

        if (blTransaction)
            command.Transaction = transaction;

        if (null != cmdParms)
            CreateDBParameters(cmdParms);
    }




    #endregion




    #endregion

    #region PARAMETER METHODS

    #region OBJECT BASED

    /// <summary>
    ///Description	    :	This function is used to Create Parameters for the Command For Execution
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	2-Dimensional Parameter Array
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    private void CreateDBParameters(object[,] colParameters)
    {
        for (int i = 0; i < colParameters.Length / 2; i++)
        {
            parameter = command.CreateParameter();
            parameter.ParameterName = colParameters[i, 0].ToString();
            parameter.Value = colParameters[i, 1];
            command.Parameters.Add(parameter);
              
        }
    }

    #endregion

    #region STRUCTURE BASED

    /// <summary>
    ///Description	    :	This function is used to Create Parameters for the Command For Execution
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	2-Dimensional Parameter Array
    ///OutPut			:	NA
    ///Comments			:	
    /// </summary>
    private void CreateDBParameters(Parameters[] colParameters)
    {
        for (int i = 0; i < colParameters.Length; i++)
        {
            Parameters oParam = (Parameters)colParameters[i];

            parameter = command.CreateParameter();
            parameter.ParameterName = oParam.paramName;
            parameter.Value = oParam.paramValue;
            parameter.Direction = oParam.paramDirection;
            parameter.DbType = oParam.paramType;   
            if (oParam.paramSize > 0) parameter.Size = oParam.paramSize;    
            command.Parameters.Add(parameter);
        }
    }

    #endregion

    #endregion

    #region EXECUTE METHODS

    #region PARAMETERLESS METHODS

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Count of Records Affected
    ///Comments			:	
    ///                     Has to be changed/removed if object based array concept is removed.
    /// </summary>
    public int ExecuteNonQuery(CommandType cmdType, string cmdText)
    {
        bool isConnectionLocal = false;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            PrepareCommand( cmdType, cmdText);
            return command.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    ///// <summary>
    /////Description	    :	This function is used to Execute the Command
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array, Clear Paramaeters
    /////OutPut			:	Count of Records Affected
    /////Comments			:	
    /////                     Has to be changed/removed if object based array concept is removed.
    ///// </summary>
    //public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText)
    //{
    //    try
    //    {
    //        PrepareCommand(blTransaction, cmdType, cmdText);
    //        int val = command.ExecuteNonQuery();

    //        return val;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (null != command)
    //            command.Dispose();
    //    }
    //}

    #endregion

    #region OBJECT BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array, Clear Parameters
    ///OutPut			:	Count of Records Affected
    ///Comments			:	
    /// </summary>
    public int ExecuteNonQuery(CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
    {
        try
        {

            OpenFactoryConnection();
            PrepareCommand(false, cmdType, cmdText, cmdParms);
            return command.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (blDisposeCommand && null != command)
                command.Dispose();
            CloseFactoryConnection();
        }
    }

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Count of Records Affected
    ///Comments			:	Overloaded method. 
    /// </summary>
    public int ExecuteNonQuery(CommandType cmdType, string cmdText, object[,] cmdParms)
    {
        return ExecuteNonQuery(cmdType, cmdText, cmdParms, true);
    }

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array, Clear Paramaeters
    ///OutPut			:	Count of Records Affected
    ///Comments			:	
    /// </summary>
    public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
    {
        try
        {

            PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
            return command.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (blDisposeCommand && null != command)
                command.Dispose();
        }
    }

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Transaction, Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Count of Records Affected
    ///Comments			:	Overloaded function. 
    /// </summary>
    public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms)
    {
        return ExecuteNonQuery(blTransaction, cmdType, cmdText, cmdParms, true);
    }

    #endregion

    #region STRUCTURE BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, Parameter Structure Array, Clear Parameters
    ///OutPut			:	Count of Records Affected
    ///Comments			:	
    /// </summary>
    public int ExecuteNonQuery(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
    {
        bool isConnectionLocal = false;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            PrepareCommand(false, cmdType, cmdText, cmdParms);
            command.Connection = connection;
            if (mblTransaction) command.Transaction = transaction;  
            return command.ExecuteNonQuery();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (blDisposeCommand && null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    /// <summary>
    ///Description	    :	This function is used to Execute the Command
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, Parameter Structure Array
    ///OutPut			:	Count of Records Affected
    ///Comments			:	Overloaded method. 
    /// </summary>
    public int ExecuteNonQuery(CommandType cmdType, string cmdText, Parameters[] cmdParms)
    {
        return ExecuteNonQuery(cmdType, cmdText, cmdParms, true);
    }

    ///// <summary>
    /////Description	    :	This function is used to Execute the Command
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Transaction, Command Type, Command Text, Parameter Structure Array, Clear Parameters
    /////OutPut			:	Count of Records Affected
    /////Comments			:	
    ///// </summary>
    //public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
    //{
    //    try
    //    {

    //        PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
    //        return command.ExecuteNonQuery();

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (blDisposeCommand && null != command)
    //            command.Dispose();
    //    }
    //}

    ///// <summary>
    /////Description	    :	This function is used to Execute the Command
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Transaction, Command Type, Command Text, Parameter Structure Array
    /////OutPut			:	Count of Records Affected
    /////Comments			:	
    ///// </summary>
    //public int ExecuteNonQuery(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms)
    //{
    //    return ExecuteNonQuery(blTransaction, cmdType, cmdText, cmdParms, true);
    //}

    #endregion

    #endregion

    #region READER METHODS

    #region PARAMETERLESS METHODS

    /// <summary>
    ///Description	    :	This function is used to fetch data using Data Reader	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Data Reader
    ///Comments			:	
    ///                     Has to be changed/removed if object based array concept is removed.
    /// </summary>
    public DbDataReader ExecuteReader(CommandType cmdType, string cmdText)
    {

        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work
        try
        {

            OpenFactoryConnection();
            PrepareCommand( cmdType, cmdText);
            DbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
            command.Parameters.Clear();
            return dr;

        }
        catch (Exception ex)
        {
            CloseFactoryConnection();
            throw ex;
        }
        finally
        {
            if (null != command)
                command.Dispose();
        }
    }

    #endregion

    #region OBJECT BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to fetch data using Data Reader	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Data Reader
    ///Comments			:	
    /// </summary>
    public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, object[,] cmdParms)
    {

        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work

        try
        {

            OpenFactoryConnection();
            PrepareCommand(false, cmdType, cmdText, cmdParms);
            DbDataReader dr = command.ExecuteReader(CommandBehavior.CloseConnection);
            command.Parameters.Clear();
            return dr;

        }
        catch (Exception ex)
        {
            CloseFactoryConnection();
            throw ex;
        }
        finally
        {
            if (null != command)
                command.Dispose();
        }
    }

    #endregion

    #region STRUCTURE BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to fetch data using Data Reader	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, Parameter AStructure Array
    ///OutPut			:	Data Reader
    ///Comments			:	
    /// </summary>
    public DbDataReader ExecuteReader(CommandType cmdType, string cmdText, Parameters[] cmdParms)
    {

        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work
        try
        {

            OpenFactoryConnection();
            PrepareCommand(false, cmdType, cmdText, cmdParms);
            return command.ExecuteReader(CommandBehavior.CloseConnection);

        }
        catch (Exception ex)
        {
            CloseFactoryConnection();
            throw ex;
        }
        finally
        {
            if (null != command)
                command.Dispose();
        }
    }

    #endregion

    #endregion

    #region ADAPTER METHODS
    
    #region PARAMETERLESS METHODS
        
    /// <summary>
    /// Description                 : Permite solo la ejecucion de un comando
    /// Author                      : fpp
    /// </summary>
    /// <param name="cmdType"></param>
    /// <param name="cmdText"></param>
    /// <returns></returns>
    public DataTable DataAdapter(DbCommand cm)
    {

        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work
        bool isConnectionLocal = false;
        DbDataAdapter dda = null;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            dda = factory.CreateDataAdapter();
            dda.SelectCommand = cm;
            cm.CommandTimeout = timeOut;  

            //PrepareCommand(false, cm.CommandType, cmdText);
            //dda.SelectCommand = command;
            DataTable dt = new DataTable();
            dda.Fill(startRecord, maxRecord, dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }


    /// <summary>
    ///Description	    :	This function is used to fetch data using Data Adapter	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Data Set
    ///Comments			:	
    ///                     Has to be changed/removed if object based array concept is removed.
    /// </summary>

    public DataTable DataAdapter(CommandType cmdType, string cmdText)
    {

        bool isConnectionLocal = false;
        DbDataAdapter dda = null;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            dda = factory.CreateDataAdapter();
            PrepareCommand(cmdType, cmdText);
            //if (mblTransaction) command.Transaction = transaction;
            dda.SelectCommand = command;
            DataTable dt = new DataTable();
            dda.Fill(startRecord, maxRecord, dt);
            return dt;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message,ex);
        }
        finally
        {
            if (null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }


    #endregion

    #region OBJECT BASED PARAMETER ARRAY

    ///// <summary>
    /////Description	    :	This function is used to fetch data using Data Adapter	
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    /////OutPut			:	Data Set
    /////Comments			:	
    ///// </summary>
    //public DataSet DataAdapter(CommandType cmdType, string cmdText, object[,] cmdParms)
    //{

    //    // we use a try/catch here because if the method throws an exception we want to 
    //    // close the connection throw code, because no datareader will exist, hence the 
    //    // commandBehaviour.CloseConnection will not work
    //    bool isConnectionLocal = false;
    //    DbDataAdapter dda = null;
    //    try
    //    {
    //        if (connectionState == ConnectionState.Closed)
    //        {
    //            isConnectionLocal = true;
    //            OpenFactoryConnection();
    //        }

    //        dda = factory.CreateDataAdapter();
    //        PrepareCommand(false, cmdType, cmdText, cmdParms);

    //        dda.SelectCommand = command;
    //        DataSet ds = new DataSet();
    //        dda.Fill(ds);
    //        return ds;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (null != command)
    //            command.Dispose();
    //        if (isConnectionLocal) CloseFactoryConnection();
    //    }
    //}

    #endregion

    #region STRUCTURE BASED PARAMETER ARRAY DATASET

    /// <summary>
    ///Description	    :	This function is used to fetch data using Data Adapter	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Data Set
    ///Comments			:	
    /// </summary>
    public DataSet DataAdapter(CommandType cmdType, string cmdText, Parameters[] cmdParms)
    {

        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work
        bool isConnectionLocal = false;
        DbDataAdapter dda = null;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            dda = factory.CreateDataAdapter();
            PrepareCommand(false, cmdType, cmdText, cmdParms);

            dda.SelectCommand = command;
            DataSet ds = new DataSet();
            dda.Fill(ds, 0, maxRecord, "Tabla");
            return ds;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    #endregion




    public void DataAdapterFillSchema(CommandType cmdType, string cmdText, out DataTable dt)
    {
        DbDataAdapter dda = null;
        bool isConnectionLocal = false;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            dda = factory.CreateDataAdapter();
            PrepareCommand(cmdType, cmdText);

            dda.SelectCommand = command;
            DataSet ds = new DataSet(); 
            dda.FillSchema(ds,SchemaType.Source);
            dt = ds.Tables[0]; 
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
        finally
        {
            if (null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }



    #region STRUCTURE BASED PARAMETER ARRAY DATATABLE



    /// <summary>
    ///Description	    :	This function is used to fetch data using Data Adapter	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Data Set
    ///Comments			:	
    /// </summary>
    public void DataAdapter(CommandType cmdType, string cmdText, Parameters[] cmdParms, out DataTable dt)
    {
        // we use a try/catch here because if the method throws an exception we want to 
        // close the connection throw code, because no datareader will exist, hence the 
        // commandBehaviour.CloseConnection will not work
        DbDataAdapter dda = null;
        bool isConnectionLocal = false;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            dda = factory.CreateDataAdapter();
            PrepareCommand(false, cmdType, cmdText, cmdParms);

            dda.SelectCommand = command;
            dt = new DataTable();
            dda.Fill(dt);
        }
        catch (Exception ex)
        {
            throw new Exception (ex.Message,ex) ;
        }
        finally
        {
            if (null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    #endregion


    #endregion

    #region SCALAR METHODS

    #region PARAMETERLESS METHODS

    /// <summary>
    ///Description	    :	This function is used to invoke Execute Scalar Method	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Object
    ///Comments			:	
    /// </summary>
    public object ExecuteScalar(CommandType cmdType, string cmdText)
    {
        bool isConnectionLocal = false;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }

            PrepareCommand(cmdType, cmdText);

            object val = command.ExecuteScalar();

            return val;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    #endregion

    #region OBJECT BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to invoke Execute Scalar Method	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Object
    ///Comments			:	
    /// </summary>
    public object ExecuteScalar(CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
    {
        bool isConnectionLocal = false;
        try
        {

            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }
            PrepareCommand(false, cmdType, cmdText, cmdParms);
            return command.ExecuteScalar();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (blDisposeCommand && null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    /// <summary>
    ///Description	    :	This function is used to invoke Execute Scalar Method	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Object
    ///Comments			:	Overloaded Method. 
    /// </summary>
    public object ExecuteScalar(CommandType cmdType, string cmdText, object[,] cmdParms)
    {
        return ExecuteScalar(cmdType, cmdText, cmdParms, true);
    }

    ///// <summary>
    /////Description	    :	This function is used to invoke Execute Scalar Method	
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    /////OutPut			:	Object
    /////Comments			:	
    ///// </summary>
    //public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms, bool blDisposeCommand)
    //{
    //    try
    //    {

    //        PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
    //        return command.ExecuteScalar();

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (blDisposeCommand && null != command)
    //            command.Dispose();
    //    }
    //}

    ///// <summary>
    /////Description	    :	This function is used to invoke Execute Scalar Method	
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    /////OutPut			:	Object
    /////Comments			:	
    ///// </summary>
    //public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, object[,] cmdParms)
    //{
    //    return ExecuteScalar(blTransaction, cmdType, cmdText, cmdParms, true);
    //}

    #endregion

    #region STRUCTURE BASED PARAMETER ARRAY

    /// <summary>
    ///Description	    :	This function is used to invoke Execute Scalar Method	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Object
    ///Comments			:	
    /// </summary>
    public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
    {
        bool isConnectionLocal = false;
        try
        {
            if (connectionState == ConnectionState.Closed)
            {
                isConnectionLocal = true;
                OpenFactoryConnection();
            }

            PrepareCommand(false, cmdType, cmdText, cmdParms);
            return command.ExecuteScalar();

        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            if (blDisposeCommand && null != command)
                command.Dispose();
            if (isConnectionLocal) CloseFactoryConnection();
        }
    }

    /// <summary>
    ///Description	    :	This function is used to invoke Execute Scalar Method	
    ///Author			:	fpp
    ///Date				:	15/02/2009
    ///Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    ///OutPut			:	Object
    ///Comments			:	Overloaded Method. 
    /// </summary>
    public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms)
    {
        return ExecuteScalar(cmdType, cmdText, cmdParms, true);
    }

    ///// <summary>
    /////Description	    :	This function is used to invoke Execute Scalar Method	
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    /////OutPut			:	Object
    /////Comments			:	
    ///// </summary>
    //public object ExecuteScalar(bool blTransaction, CommandType cmdType, string cmdText, Parameters[] cmdParms, bool blDisposeCommand)
    //{
    //    try
    //    {

    //        PrepareCommand(blTransaction, cmdType, cmdText, cmdParms);
    //        return command.ExecuteScalar();

    //    }
    //    catch (Exception ex)
    //    {
    //        throw ex;
    //    }
    //    finally
    //    {
    //        if (blDisposeCommand && null != command)
    //            command.Dispose();
    //    }
    //}

    ///// <summary>
    /////Description	    :	This function is used to invoke Execute Scalar Method	
    /////Author			:	fpp
    /////Date				:	15/02/2009
    /////Input			:	Command Type, Command Text, 2-Dimensional Parameter Array
    /////OutPut			:	Object
    /////Comments			:	
    ///// </summary>
    //public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters[] cmdParms)
    //{
    //    return ExecuteScalar(cmdType, cmdText, cmdParms, true);
    //}

    #endregion

    private string ConvertCmdTextToBD(string cmdText)
    {
        return cmdText; 
        const char TOKEN_BD_NEXT = ',';
        int posicionInicial = cmdText.IndexOf(TOKEN_BD, 0);
        int posicionFinal = cmdText.Length;
        string  tokenBD = "?";
        switch (Conexion.provider)
        {
            case "System.Data.SqlClient":
                tokenBD = "@";
                break;
            case "IBM.Data.Informix":
                break;
            case "System.Data.OracleClient":
                tokenBD = ":";
                break;
                  
        }

        for (int I = posicionInicial;I < posicionFinal; I++)
        {
            int posicion = cmdText.IndexOf(TOKEN_BD_NEXT, posicionInicial);
            if (posicion > -1) cmdText = cmdText.Substring(0, posicionInicial - 1) + tokenBD + cmdText.Substring(posicion, posicionFinal - (posicion + posicionInicial));
            else cmdText = cmdText.Substring(0, posicionInicial - 1) + tokenBD;
            posicionFinal = cmdText.Length;  
            if (posicion == -1) I = posicionFinal; 
            else I = posicion; 

        }
        return cmdText; 
    }


    public void SetDateFormat()
    {
        if (connection == null) throw new Exception("Abra una Conexion antes de usar el metodo SetDateFormat.");
        //Oracle
        if (Conexion.tipoConn == 1) ExecuteNonQuery(CommandType.Text, "ALTER SESSION SET NLS_DATE_FORMAT='dd/mm/yyyy'");
        
        //SQL Server
        if (Conexion.tipoConn == 4) ExecuteNonQuery(CommandType.Text, "SET DATEFORMAT dmy ");
    }


    //public void SetDateFormat(DbConnection cn, DbTransaction tr)
    //{

    //    if (Conexion.tipoConn == 1 || Conexion.tipoConn == 4)
    //    {
    //        PrepareCommand(CommandType.Text, string.Empty);
    //        DbCommand cm = command;
    //        if (tr != null)
    //            cm.Transaction = tr;

    //        if (Conexion.tipoConn == 1) //Oracle
    //        {
    //            cm.CommandText = "ALTER SESSION SET NLS_DATE_FORMAT='dd/mm/yyyy'";
    //        }

    //        if (Conexion.tipoConn == 4) //SQL Server
    //        {
    //            cm.CommandText = "SET DATEFORMAT dmy ";

    //        }
    //        cm.ExecuteNonQuery();
    //    }

    //}

    
    public void SetLockModeToWait()
    {
        switch (Conexion.tipoConn)
        {
            case 2:       //2 = Informix
                ExecuteNonQuery(System.Data.CommandType.Text, "SET LOCK MODE TO WAIT");
                break;
        }
    }

    public void LockTable(string table)
    {
        switch (Conexion.tipoConn)
        {
            case 2:
                ExecuteNonQuery(System.Data.CommandType.Text, " LOCK TABLE " + table);
                break;
        }
    }


    public void ParamProcesos(out string hora, out DateTime fpro)
    {
        hora = String.Empty;
        fpro = DateTime.Now;
        switch (Conexion.tipoConn)
        {
            case 2:
                fpro = Convert.ToDateTime(ExecuteScalar(System.Data.CommandType.Text, "SELECT CURRENT AS DateTimeServer FROM gbpmt "));
                hora = fpro.ToString("HH:mm:ss");
                break;
            case 1: 
                hora = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00");
                break;
            case 4:
                hora = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00");
                break;
        }
    }    

    #endregion





}
