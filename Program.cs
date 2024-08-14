using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// Lớp định nghĩa một nút trong cây Huffman
public class HuffmanNode
{
    public char Character { get; set; } // Kí tự của nút
    public int Frequency { get; set; } // Tần số xuất hiện của kí tự
    public HuffmanNode LeftChild { get; set; } // Con trái của nút
    public HuffmanNode RightChild { get; set; } // Con phải của nút
}

// So sánh tần số của các nút Huffman để sắp xếp trong hàng đợi ưu tiên
public class FrequencyComparer : IComparer<HuffmanNode>
{
    // So sánh tần số của hai nút huffman
    public int Compare(HuffmanNode x, HuffmanNode y)
    {
        return x.Frequency - y.Frequency;
    }
}

//// Lớp thực hiện mã hóa
public class HuffmanCoding
{

    // Xây dựng cây Huffman từ tần số của các ký tự
    public static HuffmanNode BuildHuffmanTree(Dictionary<char, int> frequencies)
    {
        var priorityQueue = new PriorityQueue<HuffmanNode>(new FrequencyComparer());

        foreach (var kvp in frequencies)
        {
            priorityQueue.Enqueue(new HuffmanNode { Character = kvp.Key, Frequency = kvp.Value });
        }

        while (priorityQueue.Count > 1)
        {
            var left = priorityQueue.Dequeue();
            var right = priorityQueue.Dequeue();

            var parent = new HuffmanNode
            {
                Character = '\0',
                Frequency = left.Frequency + right.Frequency,
                LeftChild = left,
                RightChild = right
            };

            priorityQueue.Enqueue(parent);
        }

        return priorityQueue.Dequeue();
    }

    // Xây dựng bảng mã Huffman từ cây Huffman đã xây dựng
    public static Dictionary<char, string> BuildHuffmanCodes(HuffmanNode root)
    {
        var codes = new Dictionary<char, string>();
        BuildHuffmanCodesRecursive(root, "", codes);
        return codes;
    }

    // Xây dựng bảng mã Huffman từ cây Huffman đã được tạo ra
    private static void BuildHuffmanCodesRecursive(HuffmanNode node, string code, Dictionary<char, string> codes)
    {
        if (node == null) return;

        if (node.Character != '\0')
        {
            codes[node.Character] = code;
        }

        BuildHuffmanCodesRecursive(node.LeftChild, code + "0", codes);
        BuildHuffmanCodesRecursive(node.RightChild, code + "1", codes);
    }

    // In cây Huffman ra màn hình
    public static void PrintHuffmanTree(HuffmanNode root, string indent = "", bool isLeft = false)
    {
        if (root == null) return;

        Console.Write(indent);
        if (isLeft)
        {
            Console.Write("├─");
            indent += "│  ";
        }
        else
        {
            Console.Write("└─");
            indent += "   ";
        }

        Console.WriteLine(root.Character != '\0' ? $"{root.Character} ({root.Frequency})" : $"({root.Frequency})");

        PrintHuffmanTree(root.LeftChild, indent, true);
        PrintHuffmanTree(root.RightChild, indent, false);
    }

    // Nén chuỗi đầu vào sử dụng bảng mã Huffman
    public static string Compress(string input, Dictionary<char, string> codes)
    {
        var compressed = new StringBuilder();
        foreach (var c in input)
        {
            compressed.Append(codes[c]);
        }
        return compressed.ToString();
    }
}

public class HuffmanCodingMain
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        //Chuỗi:ADDAABBCCBAAABBCCCBBBBCDAADDEEAA
        Console.Write("Chuỗi kí tự:");
        // Nhập chuỗi
        string input = Console.ReadLine();

        Console.WriteLine();

        // Tạo một từ điển tần số từ chuỗi nhập vào
        var frequencies = new Dictionary<char, int>();
        foreach (var c in input)
        {
            if (!frequencies.ContainsKey(c))
            {
                frequencies[c] = 0;
            }
            frequencies[c]++;
        }

        // Xây dựng cây Huffman từ tần số của các ký tự
        var huffmanTree = HuffmanCoding.BuildHuffmanTree(frequencies);

        // Tạo bảng mã Huffman từ cây Huffman đã xây dựng
        var huffmanCodes = HuffmanCoding.BuildHuffmanCodes(huffmanTree);

        // In cây Huffman ra màn hình
        Console.WriteLine("Cây Huffman:");
        HuffmanCoding.PrintHuffmanTree(huffmanTree);

        Console.WriteLine();

        // In ra bảng mã (Kí tự, tần số, mã nén) theo chiều tăng dần tần số
        Console.WriteLine("Kí tự\tTần số\tMã Nén");
        var sortedFrequencies = frequencies.OrderBy(x => x.Value);
        foreach (var kvp in sortedFrequencies)
        {
            Console.WriteLine($"{kvp.Key}\t{frequencies[kvp.Key]}\t{huffmanCodes[kvp.Key]}");
        }

        // Mã sau khi nén chuỗi được nhập vào
        var compressed = HuffmanCoding.Compress(input, huffmanCodes);
        Console.WriteLine("\nMã nén của chuỗi:");
        Console.WriteLine(compressed);

        Console.ReadKey();
    }
}

// Lớp đại diện cho một hàng đợi ưu tiên
public class PriorityQueue<T>
{
    private List<T> data; // Danh sách các phần tử trong hàng đợi
    private IComparer<T> comparer; // Đối tượng so sánh để sắp xếp các phần tử

    // Khởi tạo hàng đợi ưu tiên với một đối tượng so sánh
    public PriorityQueue(IComparer<T> comparer)
    {
        this.data = new List<T>();
        this.comparer = comparer;
    }

    // Số lượng phần tử trong hàng đợi
    public int Count => data.Count;

    // Thêm một phần tử vào hàng đợi
    public void Enqueue(T item)
    {
        data.Add(item);
        int ci = data.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (comparer.Compare(data[ci], data[pi]) >= 0) break;
            T tmp = data[ci];
            data[ci] = data[pi];
            data[pi] = tmp;
            ci = pi;
        }
    }

    // Loại bỏ và trả về phần tử có tần số nhỏ nhất trong hàng đợi
    public T Dequeue()
    {
        if (data.Count == 0)
            throw new InvalidOperationException("Queue is empty");

        T frontItem = data[0];
        int lastItemIndex = data.Count - 1;
        data[0] = data[lastItemIndex];
        data.RemoveAt(lastItemIndex);

        --lastItemIndex;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > lastItemIndex) break;
            int rc = ci + 1;
            if (rc <= lastItemIndex && comparer.Compare(data[rc], data[ci]) < 0)
                ci = rc;
            if (comparer.Compare(data[pi], data[ci]) <= 0) break;
            T tmp = data[pi];
            data[pi] = data[ci];
            data[ci] = tmp;
            pi = ci;
        }
        return frontItem;
    }

    // Thuộc tính Min để lấy phần tử có tần số nhỏ nhất trong hàng đợi
    public T Min => data[0];
}
