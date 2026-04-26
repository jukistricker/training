import { useState, useMemo, useEffect } from "react";

export function usePagination<T>(data: T[], itemsPerPage: number = 10) {
  const [currentPage, setCurrentPage] = useState(1);

  useEffect(() => {
    setCurrentPage(1);
  }, [data.length]);

  const paginationData = useMemo(() => {
    const totalItems = data.length;
    const totalPages = Math.ceil(totalItems / itemsPerPage);
    const lastIndex = currentPage * itemsPerPage;
    const firstIndex = lastIndex - itemsPerPage;
    const currentItems = data.slice(firstIndex, lastIndex);

    return {
      totalItems,
      totalPages,
      firstIndex,
      lastIndex,
      currentItems,
    };
  }, [data, currentPage, itemsPerPage]);

  return {
    currentPage,
    setCurrentPage,
    ...paginationData,
  };
}