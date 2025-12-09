Imports MySql.Data.MySqlClient
Imports System.Data

Public Class Form1


    Dim connectionString As String =
    "Server=localhost;Port=3306;Database=library_db;Uid=libraryuser;Pwd=library123;SslMode=Disabled;"


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cmbCategory.Items.AddRange({"Fiction", "Non-Fiction", "Science", "History", "Children", "Education", "Novel", "Comics"})
        TestConnection()
        LoadData()
    End Sub

    Private Sub TestConnection()
        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                MessageBox.Show("Connected to MySQL successfully.")
            End Using
        Catch ex As Exception
            MessageBox.Show("MySQL Connection FAILED: " & ex.Message)
        End Try
    End Sub

    Private Sub LoadData()
        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                Dim sql As String = "SELECT id, title, author, category, availability FROM books WHERE is_deleted=0"
                Dim adapter As New MySqlDataAdapter(sql, conn)
                Dim table As New DataTable()
                adapter.Fill(table)
                dgvBooks.DataSource = table
            End Using
        Catch ex As MySqlException
            MessageBox.Show("Database connection failed: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Error loading data: " & ex.Message)
        End Try
    End Sub

    Private Sub ClearFields()
        txtID.Clear()
        txtTitle.Clear()
        txtAuthor.Clear()
        cmbCategory.Text = ""
        txtAvailability.Clear()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If txtID.Text = "" Or txtTitle.Text = "" Then
            MessageBox.Show("ID and Title are required.")
            Return
        End If

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                Dim sql As String = "INSERT INTO books (id, title, author, category, availability) VALUES (@id,@title,@author,@cat,@avail)"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", Integer.Parse(txtID.Text))
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text)
                    cmd.Parameters.AddWithValue("@author", txtAuthor.Text)
                    cmd.Parameters.AddWithValue("@cat", cmbCategory.Text)
                    cmd.Parameters.AddWithValue("@avail", txtAvailability.Text)
                    cmd.ExecuteNonQuery()
                End Using
            End Using

            LoadData()
            ClearFields()
            MessageBox.Show("Book added successfully.")
        Catch ex As MySqlException
            MessageBox.Show("Database error: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                Dim sql As String = "UPDATE books SET title=@title, author=@author, category=@cat, availability=@avail WHERE id=@id AND is_deleted=0"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text)
                    cmd.Parameters.AddWithValue("@author", txtAuthor.Text)
                    cmd.Parameters.AddWithValue("@cat", cmbCategory.Text)
                    cmd.Parameters.AddWithValue("@avail", txtAvailability.Text)
                    cmd.Parameters.AddWithValue("@id", Integer.Parse(txtID.Text))

                    Dim rows = cmd.ExecuteNonQuery()
                    If rows = 0 Then
                        MessageBox.Show("Record not found.")
                    Else
                        MessageBox.Show("Book updated successfully.")
                    End If
                End Using
            End Using

            LoadData()
            ClearFields()
        Catch ex As MySqlException
            MessageBox.Show("Database error: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If txtID.Text = "" Then
            MessageBox.Show("Enter ID to delete.")
            Return
        End If

        Try
            Using conn As New MySqlConnection(connectionString)
                conn.Open()
                Dim sql As String = "UPDATE books SET is_deleted=1 WHERE id=@id"
                Using cmd As New MySqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@id", Integer.Parse(txtID.Text))
                    Dim rows = cmd.ExecuteNonQuery()
                    If rows = 0 Then
                        MessageBox.Show("Record not found.")
                    Else
                        MessageBox.Show("Book deleted successfully.")
                    End If
                End Using
            End Using

            LoadData()
            ClearFields()
        Catch ex As MySqlException
            MessageBox.Show("Database error: " & ex.Message)
        Catch ex As Exception
            MessageBox.Show("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub dgvBooks_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvBooks.CellClick
        If e.RowIndex < 0 Then Return
        txtID.Text = dgvBooks.Rows(e.RowIndex).Cells(0).Value.ToString()
        txtTitle.Text = dgvBooks.Rows(e.RowIndex).Cells(1).Value.ToString()
        txtAuthor.Text = dgvBooks.Rows(e.RowIndex).Cells(2).Value.ToString()
        cmbCategory.Text = dgvBooks.Rows(e.RowIndex).Cells(3).Value.ToString()
        txtAvailability.Text = dgvBooks.Rows(e.RowIndex).Cells(4).Value.ToString()
    End Sub

    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        ClearFields()
    End Sub

End Class
