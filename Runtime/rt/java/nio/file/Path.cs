//***************************************************
//* This file was generated by JSharp
//***************************************************
namespace java.nio.file
{
    public partial interface Path : global::java.lang.Comparable<Path>, global::java.lang.Iterable<Path>, Watchable
    {
        bool endsWith(Path prm1);
        bool endsWith(global::java.lang.String prm1);
        bool equals(global::System.Object prm1);
        Path getName(int prm1);
        int hashCode();
        global::java.util.Iterator<Path> iterator();
        Path normalize();
        WatchKey register(WatchService prm1, WatchEvent_Kind<global::System.Object>[] prm2);
        WatchKey register(WatchService prm1, WatchEvent_Kind<global::System.Object>[] prm2, WatchEvent_Modifier[] prm3);
        Path relativize(Path prm1);
        Path resolve(global::java.lang.String prm1);
        Path resolve(Path prm1);
        Path resolveSibling(Path prm1);
        Path resolveSibling(global::java.lang.String prm1);
        bool startsWith(global::java.lang.String prm1);
        bool startsWith(Path prm1);
        Path subpath(int prm1, int prm2);
        Path toAbsolutePath();
        global::java.io.File toFile();
        Path toRealPath(LinkOption[] prm1);
        global::java.lang.String toString();
        global::java.net.URI toUri();
        Path  FileName { get;}
        global::java.nio.file.FileSystem  FileSystem { get;}
        bool  IsAbsolute { get;}
        int  NameCount { get;}
        Path  Parent { get;}
        Path  Root { get;}
    }
}
