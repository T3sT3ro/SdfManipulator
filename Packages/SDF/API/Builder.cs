namespace API {
    public interface Builder<out O, in I> {
        public O Build(I input);
    }
}
