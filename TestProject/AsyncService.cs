namespace TestProject
{
    public class AsyncService
    {
        public async Task<int> GetNumberAsync()
        {
            await Task.Delay(100);
            return 42;
        }

        public async Task<string> GetTextAsync()
        {
            await Task.Delay(50);
            return "hello";
        }
    }
}