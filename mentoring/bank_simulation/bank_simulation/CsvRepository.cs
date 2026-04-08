using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bank_simulation;

public class CsvRepository<T> where T : new()
{
    public readonly string _filePath;
    public readonly string _header;
    public readonly Func<string[], T> _mapFromCsv;
    public readonly Func<T, string> _mapToCsv;

    public CsvRepository(string filePath, string header, Func<string[], T> fromCsv, Func<T, string> toCsv)
    {
        _filePath = filePath;
        _header = header;
        _mapFromCsv = fromCsv;
        _mapToCsv = toCsv;

        Program.createCsv(filePath, header);
    }

    public List<T> GetAll()
    {
        if (!File.Exists(_filePath)) return new List<T>();
        return File.ReadAllLines(_filePath)
                   .Skip(1)
                   .Select(line => _mapFromCsv(line.Split(',')))
                   .ToList();
    }

    public T Get(Func<T, bool> predicate) => GetAll().FirstOrDefault(predicate);

    public void Add(T entity)
    {
        File.AppendAllLines(_filePath, new[] { _mapToCsv(entity) });
    }

    public void Update(Func<T, bool> predicate, T updatedEntity)
    {
        var all = GetAll();
        var index = all.FindIndex(new Predicate<T>(predicate));
        if (index != -1)
        {
            all[index] = updatedEntity;
            var lines = new List<string> { _header };
            lines.AddRange(all.Select(_mapToCsv));
            File.WriteAllLines(_filePath, lines);
        }
    }


}

