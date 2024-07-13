import { useEffect, useRef, useState } from "react";

export default function ModalTemplate({ isOpen, onClose, children, className, onkeydown }) {
    const [isModalOpen,setModalOpen] = useState(isOpen);
    const modalRef =useRef(null);

useEffect(()=>{
    setModalOpen(isOpen);
},[isOpen])
useEffect(()=>{
    const modalElement = modalRef.current;
    if (modalElement) {
        if (isModalOpen) {
            modalElement.showModal();
        }
        else {
            modalElement.close();
        }
    }

},[isModalOpen])

function handleCloseModal() {
    setModalOpen(false);
}



    return (
        <dialog ref={modalRef} className={className} onKeyDown={onkeydown}>

            {children}
        </dialog>
    )
}