using System;
using System.Data;
using System.Data.SqlClient;

class DataAccess
{
    static string connectionString =
        "Data Source=DESKTOP-0RGINKQ\\SQLEXPRESS;Initial Catalog=Library;Integrated Security=True;Connect Timeout=30;Encrypt=False;";

    SqlConnection conn = new SqlConnection(connectionString);
    SqlDataAdapter adapter;
    DataSet ds;

    public void Start()
    {
        while (true)
        {
            int choice = Menu(new string[]
            {
                "Books",
                "Exit"
            });

            if (choice == 0)
                Books();
            else
                break;
        }
    }

    void Books()
    {
        adapter = new SqlDataAdapter(
            "SELECT * FROM BooksLibrary",
            conn);

        ds = new DataSet();
        adapter.Fill(ds, "Books");

        DataTable table = ds.Tables["Books"];

        if (table.Rows.Count == 0)
        {
            Console.Clear();
            Console.WriteLine("Book yoxdur!");
            Console.ReadLine();
            return;
        }

        string[] books = new string[table.Rows.Count];

        for (int i = 0; i < table.Rows.Count; i++)
            books[i] = table.Rows[i]["Name"].ToString();

        int choice = Menu(books);

        DataRow book = table.Rows[choice];

        int action = Menu(new string[]
        {
            "Update",
            "Delete",
            "Geri"
        });

        if (action == 0)
            Update(book);

        else if (action == 1)
            Delete(book);
    }

    void Update(DataRow book)
    {
        string[] columns =
        {
            "Name",
            "Pages",
            "YearPress",
            "Comment",
            "Quantity",
            "Geri"
        };

        int choice = Menu(columns);

        if (choice == 5)
            return;

        string column = columns[choice];

        Console.Clear();

        Console.WriteLine("Kitab: " + book["Name"]);
        Console.WriteLine();
        Console.WriteLine("Indiki deyer: " + book[column]);
        Console.WriteLine();

        Console.Write("Yeni deyer: ");
        string value = Console.ReadLine();

        try
        {
            if (column == "Pages" ||
                column == "YearPress" ||
                column == "Quantity")
            {
                book[column] = int.Parse(value);
            }
            else
            {
                book[column] = value;
            }

            SqlCommandBuilder builder =
                new SqlCommandBuilder(adapter);

            adapter.Update(ds, "Books");

            Console.Clear();
            Console.WriteLine("Update edildi!");
        }
        catch (Exception ex)
        {
            Console.Clear();
            Console.WriteLine("Xeta: " + ex.Message);
        }

        Console.ReadLine();
    }

    void Delete(DataRow book)
    {
        Console.Clear();

        Console.WriteLine("Silinecek kitab: " + book["Name"]);
        Console.WriteLine();

        int choice = Menu(new string[]
        {
            "Beli, sil",
            "Xeyr, geri"
        });

        if (choice == 1)
            return;

        int id = Convert.ToInt32(book["Id"]);

        try
        {
            conn.Open();

            SqlTransaction transaction = conn.BeginTransaction();

            SqlCommand command = new SqlCommand(
                "DELETE FROM S_Cards WHERE Id_Book = @id;" +
                "DELETE FROM T_Cards WHERE Id_Book = @id;" +
                "DELETE FROM BooksLibrary WHERE Id = @id;",
                conn,
                transaction);

            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();

            transaction.Commit();

            conn.Close();

            Console.Clear();
            Console.WriteLine("Book silindi!");
        }
        catch (Exception ex)
        {
            if (conn.State == ConnectionState.Open)
                conn.Close();

            Console.Clear();
            Console.WriteLine("Xeta: " + ex.Message);
        }

        Console.ReadLine();
    }

    static int Menu(string[] items)
    {
        int selected = 0;

        while (true)
        {
            Console.Clear();

            for (int i = 0; i < items.Length; i++)
            {
                if (i == selected)
                    Console.WriteLine("> " + items[i]);
                else
                    Console.WriteLine("  " + items[i]);
            }

            ConsoleKey key =
                Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selected--;

                if (selected < 0)
                    selected = items.Length - 1;
            }

            else if (key == ConsoleKey.DownArrow)
            {
                selected++;

                if (selected >= items.Length)
                    selected = 0;
            }

            else if (key == ConsoleKey.Enter)
            {
                return selected;
            }
        }
    }
}

class Program
{
    static void Main()
    {
        DataAccess db = new DataAccess();

        db.Start();
    }
}
