var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

app.MapGet("/", () => "Hi");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();
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
                throw new Exception("Номер телефона должен состоять из цифр.");
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
                throw new Exception("Номер телефона должен состоять из цифр.");
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

public enum OrderStatus
{
    WaitingForExecution,
    InRepair,
    ReadyToIssue
}

