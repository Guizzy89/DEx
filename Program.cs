using System.Reflection.Metadata.Ecma335;
using static UpdateOrderDTO;
List<Executor> executors = new List<Executor>()
            {
                new Executor("����", "������", "79161234567"),
                new Executor("����", "������", "79201234568"),
                new Executor("������", "�������", "79301345679")
            };

Client client = new Client("������", "��������", "78912344556", "ghfjgh@gmail.com");
List<Order> repo = [
    new Order(1, new DateTime(2025, 02, 10), "Phone", "�� ��������", "�� ����������", OrderStatus.InRepair, client, executors[0] ),
    new Order(2, new DateTime(2025, 02, 13), "TV", "�� ��������", "�� ����������", OrderStatus.WaitingForExecution, null, null )];

var builder = WebApplication.CreateBuilder();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

string message = "";

app.MapGet("/", () => "Hi");
app.MapGet("orders", (int param = 0) => 
{ 
    string buffer = message;
    message = "";
    if (param != 0)
        return new {repo= repo.FindAll(x => x.OrderNumber == param), message = buffer};
    return new {repo,  message = buffer};
});

app.MapGet("create", ([AsParameters] Order dto) => repo.Add(dto));
app.MapGet("Update", ([AsParameters] UpdateOrderDTO dto) => 
{ 
    var order = repo.Find(x => x.OrderNumber == dto.OrderNumber);
    if (order == null)
        return Results.NotFound();
    if (dto.Status != order.Status)
    {
        order.Status = dto.Status;
        message += $"������ ������ �{order.OrderNumber} ������\n";
        if (order.Status == OrderStatus.WaitingForExecution)
            message += $"������ � {order.OrderNumber} ������� �����������\n";
        if (order.Status == OrderStatus.InRepair)
            message += $"������ � {order.OrderNumber} � �������\n";
        if (order.Status == OrderStatus.ReadyToIssue)
        {
            message += $"������ � {order.OrderNumber} ��������� � ������ � ������\n";
            order.EndDate = DateOnly.FromDateTime(DateTime.Now);
        }   
    }
    return Results.Ok(order);
});

app.Run();



public class Client
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    private string _phoneNumber = "";
    public string PhoneNumber
    {
        get { return _phoneNumber; }
        set
        {
            ValidatePhoneNumber(value);
            _phoneNumber = value;
        }
    }
    public string? Email { get; set; }

    public Client(string firstName, string lastName, string phoneNumber, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    private void ValidatePhoneNumber(string phoneNumber)
    {
        foreach (char symbol in phoneNumber)
        {
            if (char.IsLetter(symbol))
                throw new Exception("����� �������� ������ �������� �� ����.");
        }
    }
}

public class Executor
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    private string _phoneNumber = "";
    public string PhoneNumber
    {
        get { return _phoneNumber; }
        set
        {
            ValidatePhoneNumber(value);
            _phoneNumber = value;
        }
    }

    public Executor(string firstName, string lastName, string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }

    private void ValidatePhoneNumber(string phoneNumber)
    {
        foreach (char symbol in phoneNumber)
        {
            if (!char.IsDigit(symbol))
                throw new Exception("����� �������� ������ �������� �� ����.");
        }
    }
}

public class Order
{
    public int OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public string Device { get; set; }
    public string ProblemType { get; set; }
    public string Description { get; set; }
    public OrderStatus Status { get; set; }
    public Client? Client { get; set; }
    public Executor? Executor { get; set; }
    public DateOnly? EndDate { get; set; } = null;
    public List<string> Comments { get; set; } = [];

    public Order(int orderNumber, DateTime orderDate, string device, string problemType, string description, OrderStatus status, Client client, Executor executor)
    {
        OrderNumber = orderNumber;
        OrderDate = orderDate;
        Device = device;
        ProblemType = problemType;
        Description = description;
        Status = status;
        Client = client;
        Executor = executor;
    }

    public void ChangeStatus()
    {
        switch (Status)
        {
            case OrderStatus.WaitingForExecution:
                Status = OrderStatus.InRepair;
                break;
            case OrderStatus.InRepair:
                Status = OrderStatus.ReadyToIssue;
                break;
            case OrderStatus.ReadyToIssue:
                Status = OrderStatus.WaitingForExecution;
                break;
        }
    }
}

record class UpdateOrderDTO(int OrderNumber, OrderStatus Status, Client Client, Executor Executor, string Comment = "");

public enum OrderStatus
{
    WaitingForExecution,
    InRepair,
    ReadyToIssue
}

