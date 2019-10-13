public interface IBlockArray
{


     int Lowest(int x, int z);

     Block Get(int x, int y, int z);

     void Set(int x, int y, int z, Block block);

     int GetWidth();

     int GetHeight();

     int GetDepth();
}
