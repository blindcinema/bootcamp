

import { useRef, useState } from "react";
import Form from "react-bootstrap/Form";

export default function SortingForm(props) {
    const formRef = useRef(null);

    const [form,setForm] = useState("Asc");
    function handleChange(e) {
        e.preventDefault();
        setForm(e.target.value);
        //formRef.current.submit();
        props.onChange(form);
        
    }

    return (
        <>
            <form ref={formRef} onChange={handleChange} >
                <Form.Select className='w-min' defaultValue={"Asc"} >
                    <option value="Asc">Asc.</option>
                    <option value="Desc">Desc.</option>
                </Form.Select>
            </form>
        
        </>
    )
}