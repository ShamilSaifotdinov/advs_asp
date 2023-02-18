import { Link } from "react-router-dom"

function Adv({ID, Name, Location, Discription, Price}) {
    return (
        <div className="adv">
            <Link to={"objects/" + ID} target="_blank" ><h1>{Name}</h1></Link>
            <p>{Location}</p>
            <p>{Discription}</p>
            <h2>{Price.toFixed(2)} руб.</h2>
        </div>
    )
        
}

export default Adv;