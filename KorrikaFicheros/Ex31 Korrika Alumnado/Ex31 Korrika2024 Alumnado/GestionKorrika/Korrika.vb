Imports System.IO
Imports Entidades

Public Class Korrika
    Public ReadOnly Property Cambios As Boolean = False
    Public Property DatosGenerales As DatosGeneralesKorrika
    Private Property _Provincias As New List(Of String) From {"araba", "gipuzkoa", "nafarroa", "bizkaia", "zuberoa", "nafarra behera", "lapurdi"}
    Public ReadOnly Property Provincias
        Get
            Return _Provincias.AsReadOnly
        End Get
    End Property
    Private Property _Kilometros As New List(Of Kilometro)
    Public ReadOnly Property Kilometros
        Get
            Return _Kilometros.AsReadOnly
        End Get
    End Property
    Private Property _TotalRecaudado As Decimal
    Public ReadOnly Property TotalRecaudado
        Get
            Return _TotalRecaudado
        End Get
    End Property
    Private Function LeerKorrika(num As Integer) As String
        If Not File.Exists($".\Ficheros\Korrika{num}.txt") Then Return $"No existe la Korrika {num}"
        Dim lineasFichero() As String = File.ReadAllLines($".\Ficheros\Korrika{num}.txt")
        Dim datosKorrika() As String = lineasFichero(0).Split("*")
        DatosGenerales = New DatosGeneralesKorrika(datosKorrika(0), datosKorrika(1), datosKorrika(2), datosKorrika(3), datosKorrika(4), datosKorrika(5))
        CrearKilometros(DatosGenerales.CantKms)
        Dim KmKorrika() As String
        For i = 1 To lineasFichero.Length - 1
            KmKorrika = lineasFichero(i).Split("*")
            If KmKorrika.Length > 4 Then
                DefinirKm(KmKorrika(0), KmKorrika(1), KmKorrika(2), KmKorrika(3))
                PatrocinarKilometro(KmKorrika(0), KmKorrika(4), KmKorrika(5))
            ElseIf KmKorrika.Length > 1 Then
                DefinirKm(KmKorrika(0), KmKorrika(1), KmKorrika(2), KmKorrika(3))
            End If
        Next
        _Cambios = False
        Return ""
    End Function
    Public Function GrabarCambios() As String
        Dim lineas() As String = {}
        Array.Resize(lineas, 1)
        lineas(lineas.Length - 1) = $"{DatosGenerales.NKorrika}*{DatosGenerales.Anyo}*{DatosGenerales.Eslogan}*{DatosGenerales.FechaInicio.ToShortDateString}*{DatosGenerales.FechaFin.ToShortDateString}*{DatosGenerales.CantKms}"
        For Each km As Kilometro In Kilometros
            Array.Resize(lineas, lineas.Length + 1)
            If TypeOf km IsNot KilometroFinanciado Then
                If km.Direccion Is Nothing Then
                    lineas(lineas.Length - 1) = $"{km.NumKm}"
                Else
                    lineas(lineas.Length - 1) = $"{km.NumKm}*{km.Direccion}*{km.Localidad}*{km.Provincia}"
                End If
            Else
                Dim kmFin As KilometroFinanciado = TryCast(km, KilometroFinanciado)
                lineas(lineas.Length - 1) = $"{kmFin.NumKm}*{kmFin.Direccion}*{kmFin.Localidad}*{kmFin.Provincia}*{kmFin.Organizacion}*{kmFin.Euros}"
            End If
        Next
        File.WriteAllLines($".\Ficheros\Korrika{DatosGenerales.NKorrika}.txt", lineas)
        Return ""
    End Function
    Private Sub TotalRecaudadoCalculo(euros As Decimal)
        _TotalRecaudado += euros
    End Sub
    Public Sub New(datosGeneralesKorrika As DatosGeneralesKorrika, ByRef msgError As String)
        If File.Exists($".\Ficheros\Korrika{datosGeneralesKorrika.NKorrika}.txt") Then msgError = $"Ya existe la Korrika {datosGeneralesKorrika.NKorrika}"
        DatosGenerales = datosGeneralesKorrika
        CrearKilometros(DatosGenerales.CantKms)
        GrabarCambios()
    End Sub
    Public Sub New(numKorrika As Integer, ByRef msgError As String)
        msgError = LeerKorrika(numKorrika)
    End Sub
    Private Sub CrearKilometros(cantKm As Integer)
        _Kilometros.Clear()
        For i = 1 To cantKm
            _Kilometros.Add(New Kilometro(i))
        Next
    End Sub
    Public Overrides Function ToString() As String
        Return DatosGenerales.ToString
    End Function

    Public Function DefinirKm(numKm As Integer, direccion As String, localidad As String, provincia As String) As String
        Dim msg As String = DefinirKm(New Kilometro(numKm, direccion, localidad, provincia))
        Return msg
    End Function
    Public Function DefinirKm(kilometro As Kilometro) As String
        If kilometro Is Nothing Then
            Return "El kilometro no existe"
        End If
        If String.IsNullOrWhiteSpace(kilometro.Direccion) Then
            Return "La dirección no puede quedar vacia"
        End If
        If String.IsNullOrWhiteSpace(kilometro.Localidad) Then
            Return "La localidad no puede quedar vacia"
        End If
        If String.IsNullOrWhiteSpace(kilometro.Provincia) Then
            Return "La provincia no puede quedar vacia"
        End If
        If Not _Provincias.Contains(kilometro.Provincia.ToLower) Then
            Return $"No existe la provincia {kilometro.Provincia}"
        End If
        Dim posKm As Integer = _Kilometros.IndexOf(kilometro)
        If posKm = -1 Then
            Return $"No existe el kilometro {kilometro.NumKm}"
        End If
        For Each km In _Kilometros
            If kilometro.Direccion = km.Direccion AndAlso kilometro.Localidad = km.Localidad Then
                Return $"El kilómetro número {km.NumKm} ya c comienza en la dirección {km.Direccion} de {km.Provincia}"
            End If
        Next
        _Kilometros(posKm) = kilometro
        _Cambios = True
        Return ""
    End Function

    Public Function PatrocinarKilometro(numKm As Integer, organizacion As String, euros As Decimal) As String
        If euros <= 0 Then
            Return "Para patrocinar el kilometro tienes que aportar dinero"
        End If
        Dim posKm As Integer = _Kilometros.IndexOf(New Kilometro(numKm))
        If posKm = -1 Then
            Return $"No existe el kilometro {numKm}"
        End If
        Dim kmAux As KilometroFinanciado = TryCast(_Kilometros(posKm), KilometroFinanciado)
        If kmAux IsNot Nothing Then
            Return $"El kilómetro número {numKm} ya está financiado por { kmAux.Organizacion}"
        End If
        If String.IsNullOrWhiteSpace(organizacion) Then
            Return $"Tiene que haber una organizacion patrocinadora"
        End If
        Dim organizacionYaEstaba As Boolean = False
        Dim kmFinanciadosOrg As Integer
        For Each km In _Kilometros
            Dim kmKmFinanciado As KilometroFinanciado = TryCast(km, KilometroFinanciado)
            If kmKmFinanciado IsNot Nothing Then
                If kmKmFinanciado.Organizacion.ToLower = organizacion.ToLower Then
                    organizacionYaEstaba = True
                    kmFinanciadosOrg += 1
                End If
            End If
        Next
        _Kilometros(posKm) = New KilometroFinanciado(_Kilometros(posKm), organizacion, euros)
        TotalRecaudadoCalculo(euros)
        _Cambios = True
        If organizacionYaEstaba Then
            Return $"La organización {organizacion} financia el kilómetro {numKm}, aunque ya había financiado otros {kmFinanciadosOrg} kilómetros"
        End If
        Return $"La organización {organizacion} financia el kilómetro {numKm}"
    End Function
    Public Function KilometrosLibreProvincia(provincia As String) As List(Of Kilometro)
        If Not _Provincias.Contains(provincia) Then
            Return Nothing
        End If
        Dim kmLibres As New List(Of Kilometro)
        For Each km In _Kilometros
            If km.Provincia.ToLower = provincia.ToLower Then
                If TypeOf km IsNot KilometroFinanciado Then
                    kmLibres.Add(km)
                End If
            End If
        Next
        Return kmLibres
    End Function
End Class
