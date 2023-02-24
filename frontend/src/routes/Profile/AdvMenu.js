//import { Link } from "react-router-dom"
import useQuery from "../../hooks/hook.http.js"

function AdvMenu({advId, updateParent}) {    
    const [ http, loading, errorState ] = useQuery();
    const handleDelete = async () => {
        console.log(advId)
        const res = await http(`/advs/${advId}`, "DELETE", {}, {
            "Accept": "application/json",
            "Authorization": "Bearer " + localStorage.getItem('token')
        })
        if (res) {
            console.log(res);
            alert(res.message);
            updateParent();
        }
        else {
            console.error(errorState);
            alert(errorState);
        }
    }

    return (
        <div className="advMenu">
            {/* <button className="header_btn">Редактировать</button> */}
            <button className="header_btn" onClick={handleDelete}>Удалить</button>
        </div>
    )
        
}

export default AdvMenu;