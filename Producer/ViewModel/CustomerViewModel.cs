namespace Producer.ViewModel;

public class CustomerViewModel
{
    public Guid Id { get; private set; }
    public string Name { get; set; }
    public int Age { get; set; }

    public CustomerViewModel()
    {
        Id = Guid.NewGuid();
    }
}