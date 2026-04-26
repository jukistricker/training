import React, { ReactNode } from "react";

//em phải viết thêm 1 cái base dialog khi nhận ra rằng mình đã viết quá nhiều dialog
interface BaseDialogProps {
  isOpen: boolean;
  onClose: () => void;
  title: string;
  children: ReactNode; // truyền component khác vào
  footer?: ReactNode;   // Option để truyền các nút bấm bên dưới
}

export const BaseDialog = ({ isOpen, onClose, title, children, footer }: BaseDialogProps) => {
  if (!isOpen) return null;

  return (
    <div className="modal fade show d-block account-details-modal">
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg">
          {/* Header */}
          <div className="modal-header border-bottom-0">
            <h5 className="modal-title fw-bold">{title}</h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <div className="modal-body">
            {children}
          </div>

          {footer && (
            <div className="modal-footer border-top-0">
              {footer}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};