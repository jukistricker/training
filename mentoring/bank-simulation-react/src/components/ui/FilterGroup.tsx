export interface FilterOption<T> {
  label: string;
  value: T;
}

interface FilterGroupProps<T> {
  options: FilterOption<T>[];
  activeValue: T;
  onChange: (value: T) => void;
  className?: string;
}

export const FilterGroup = <T,>({ 
  options, 
  activeValue, 
  onChange, 
  className = "" 
}: FilterGroupProps<T>) => {
  return (
    <div className={`btn-group shadow-sm ${className}`} role="group">
      {options.map((option, index) => (
        <button
          key={index} 
          type="button"
          className={`btn btn-sm px-3 ${
            activeValue === option.value 
              ? "btn-primary shadow-none" 
              : "btn-outline-primary"
          }`}
          onClick={() => onChange(option.value)}
        >
          {option.label}
        </button>
      ))}
    </div>
  );
};