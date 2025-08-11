''' <summary>
''' Class that encapsulates most common parameters sent by DataTables plugin
''' </summary>
Public Class jQueryDataTableParamModel
    ''' <summary>
    ''' Request sequence number sent by DataTable,
    ''' same value must be returned in response
    ''' </summary>       
    Public Property sEcho() As String
        Get
            Return m_sEcho
        End Get
        Set(value As String)
            m_sEcho = Value
        End Set
    End Property
    Private m_sEcho As String

    ''' <summary>
    ''' Text used for filtering
    ''' </summary>
    Public Property sSearch() As String
        Get
            Return m_sSearch
        End Get
        Set(value As String)
            m_sSearch = Value
        End Set
    End Property
    Private m_sSearch As String

    ''' <summary>
    ''' Number of records that should be shown in table
    ''' </summary>
    Public Property iDisplayLength() As Integer
        Get
            Return m_iDisplayLength
        End Get
        Set(value As Integer)
            m_iDisplayLength = Value
        End Set
    End Property
    Private m_iDisplayLength As Integer

    ''' <summary>
    ''' First record that should be shown(used for paging)
    ''' </summary>
    Public Property iDisplayStart() As Integer
        Get
            Return m_iDisplayStart
        End Get
        Set(value As Integer)
            m_iDisplayStart = Value
        End Set
    End Property
    Private m_iDisplayStart As Integer

    ''' <summary>
    ''' Number of columns in table
    ''' </summary>
    Public Property iColumns() As Integer
        Get
            Return m_iColumns
        End Get
        Set(value As Integer)
            m_iColumns = Value
        End Set
    End Property
    Private m_iColumns As Integer

    ''' <summary>
    ''' Number of columns that are used in sorting
    ''' </summary>
    Public Property iSortingCols() As Integer
        Get
            Return m_iSortingCols
        End Get
        Set(value As Integer)
            m_iSortingCols = Value
        End Set
    End Property
    Private m_iSortingCols As Integer

    ''' <summary>
    ''' Comma separated list of column names
    ''' </summary>
    Public Property sColumns() As String
        Get
            Return m_sColumns
        End Get
        Set(value As String)
            m_sColumns = Value
        End Set
    End Property
    Private m_sColumns As String
End Class
