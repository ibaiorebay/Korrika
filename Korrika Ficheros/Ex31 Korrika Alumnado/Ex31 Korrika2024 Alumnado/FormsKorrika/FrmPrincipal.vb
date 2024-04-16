﻿Imports Entidades
Imports GestionKorrika

Public Class FrmPrincipal
    Private Sub btnCrearKorrika_Click(sender As Object, e As EventArgs) Handles btnCrearKorrika.Click
        Dim comprobarNumeros As Integer
        If String.IsNullOrWhiteSpace(txtNumKorrika.Text) Then
            MessageBox.Show("El número de la korrika no puede quedar vacio")
            Exit Sub
        End If
        If Not Integer.TryParse(txtNumKorrika.Text, comprobarNumeros) OrElse comprobarNumeros <= 0 Then
            MessageBox.Show("El número de la korrika tiene que ser un número positivo")
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(txtAnyo.Text) Then
            MessageBox.Show("El año de la korrika no puede quedar vacio")
            Exit Sub
        End If
        If Not Integer.TryParse(txtAnyo.Text, comprobarNumeros) OrElse comprobarNumeros <= 2000 Then
            MessageBox.Show("El año de la korrika tiene que ser un año mayor al 2000")
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(txtEslogan.Text) Then
            MessageBox.Show("El eslogan no puede quedar vacio")
            Exit Sub
        End If
        Dim comprobarFechas As Date
        If String.IsNullOrWhiteSpace(txtFechaInicio.Text) Then
            MessageBox.Show("La fecha de inicio no puede quedar vacia")
            Exit Sub
        End If
        If Not Date.TryParse(txtFechaInicio.Text, comprobarFechas) Then
            MessageBox.Show("La fecha de inicio tiene que ser realmente una fecha")
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(txtFechaFin.Text) Then
            MessageBox.Show("La fecha de fin no puede quedar vacia")
            Exit Sub
        End If
        If Not Date.TryParse(txtFechaFin.Text, comprobarFechas) OrElse txtFechaInicio.Text >= comprobarFechas Then
            MessageBox.Show("La fecha de fin tiene que ser realmente una fecha mayor a la de inicio")
            Exit Sub
        End If
        If String.IsNullOrWhiteSpace(txtNumKorrika.Text) Then
            MessageBox.Show("Tiene que haber un número de kilometros")
            Exit Sub
        End If
        If Not Integer.TryParse(txtNumKorrika.Text, comprobarNumeros) OrElse comprobarNumeros <= 0 Then
            MessageBox.Show("El número de kilometros tiene que ser un número positivo")
            Exit Sub
        End If
        If korrika IsNot Nothing Then
            If MessageBox.Show($"Ya has creado una korrika {korrika.ToString}", "Atencion", MessageBoxButtons.YesNo) = DialogResult.No Then
                Exit Sub
            End If
        End If
        Dim nuevaKorrika As DatosGeneralesKorrika
        nuevaKorrika.NKorrika = txtNumKorrika.Text
        nuevaKorrika.Anyo = txtAnyo.Text
        nuevaKorrika.Eslogan = txtEslogan.Text
        nuevaKorrika.FechaInicio = txtFechaInicio.Text
        nuevaKorrika.FechaFin = txtFechaFin.Text
        nuevaKorrika.CantKms = txtCantKms.Text
        Dim msgError As String = ""
        korrika = New Korrika(nuevaKorrika, msgError)
    End Sub

    Private Sub DefinirKmToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DefinirKmToolStripMenuItem.Click, FinanciarKmToolStripMenuItem.Click
        If korrika Is Nothing Then
            MessageBox.Show("Antes debes crear una korrika")
            Exit Sub
        End If
        If sender Is DefinirKmToolStripMenuItem Then
            Dim frmDefinirKm As New FrmDefinirKms
            frmDefinirKm.Show()
        End If
        If sender Is FinanciarKmToolStripMenuItem Then
            Dim frmFinanciacion As New FrmFinanciacion
            frmFinanciacion.Show()
        End If

    End Sub


    Private Sub FinalizarToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles FinalizarToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub btnKorrikaExistente_Click(sender As Object, e As EventArgs) Handles btnKorrikaExistente.Click
        Dim msgerror As String = ""
        If String.IsNullOrEmpty(txtNumKorrika.Text) Then
            MessageBox.Show("Tienes que escribir un numero de korrika")
        End If
        korrika = New Korrika(txtNumKorrika.Text, msgerror)
        If msgerror IsNot "" Then
            MessageBox.Show(msgerror)
        End If
        txtAnyo.Text = korrika.DatosGenerales.Anyo
        txtEslogan.Text = korrika.DatosGenerales.Eslogan
        txtFechaInicio.Text = korrika.DatosGenerales.FechaInicio
        txtFechaFin.Text = korrika.DatosGenerales.FechaFin
        txtCantKms.Text = korrika.DatosGenerales.CantKms
    End Sub
End Class