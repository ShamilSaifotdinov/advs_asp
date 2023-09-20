function AdvContain({advId, name, location, discription, price}) {
    return (
        <div className="advContain">
            <a href={"/objects/" + advId} target="_blank" ><h1>{name}</h1></a>
            <p>{location}</p>
            <p>{discription}</p>
            <h2>{price.toFixed(2)} руб.</h2>
        </div>
    )
        
}

export default AdvContain;