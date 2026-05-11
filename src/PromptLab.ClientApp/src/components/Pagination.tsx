interface PaginationProps {
  pageNumber: number;
  pageSize: number;
  totalRows: number;
  onPageChange: (page: number) => void;
}

export function Pagination({ pageNumber, pageSize, totalRows, onPageChange }: PaginationProps) {
  const totalPages = Math.max(1, Math.ceil(totalRows / pageSize));
  if (totalPages <= 1) return null;

  return (
    <div className="flex items-center justify-center gap-2 py-4">
      <button
        type="button"
        disabled={pageNumber <= 1}
        onClick={() => onPageChange(pageNumber - 1)}
        className="px-3 py-1.5 text-sm rounded-lg bg-gray-800 text-gray-300 disabled:opacity-40 border border-gray-700"
      >
        Anterior
      </button>
      <span className="text-sm text-gray-400">
        Página {pageNumber} de {totalPages}
      </span>
      <button
        type="button"
        disabled={pageNumber >= totalPages}
        onClick={() => onPageChange(pageNumber + 1)}
        className="px-3 py-1.5 text-sm rounded-lg bg-gray-800 text-gray-300 disabled:opacity-40 border border-gray-700"
      >
        Siguiente
      </button>
    </div>
  );
}
