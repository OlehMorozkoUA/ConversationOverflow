namespace Models.Interfaces
{
    interface IIdentity<T> where T: struct
    {
        T Id { get; set; }
    }
}
