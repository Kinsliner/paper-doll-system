namespace Ez.Input
{
    public abstract class Inputter
    {
        public abstract bool IsCheck();

        public virtual T ReadInput<T>()
        {
            return default;
        }
    }

}

