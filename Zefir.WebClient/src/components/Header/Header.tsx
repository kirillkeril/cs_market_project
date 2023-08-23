import logo from "../../assets/logo/brandlogo.svg";
import accountIcon from "../../assets/icons/account.svg";
import cartIcon from "../../assets/icons/cart.svg";
import burgerMenu from "../../assets/icons/burger-menu.svg";
import crossIcon from "../../assets/icons/cross.svg";

import styles from "./Header.module.css";
import {useEffect, useState} from "react";

export const Header = () => {
    const [isMenuOpen, setMenuOpen] = useState<boolean>(false);
    const [width, ] = useState<number>(screen.width);

    useEffect(() => {
        if(width >= 800) setMenuOpen(true);
        else setMenuOpen(false);
    }, [width]);
    return (
        <div className={styles.head}>
            <button
                onClick={() => setMenuOpen(!isMenuOpen)}
                className={
                    !isMenuOpen
                        ? styles.burgerMenu
                        : styles.burgerMenu + " " + styles.burgerMenuOpen
                }
            >
                <img src={isMenuOpen ? crossIcon : burgerMenu} alt={"burger-menu"}/>
            </button>

            <header className={styles.header}>
                <a className={styles.logo} href={"/"}>
                    <img src={logo} alt={"zefir"}/>
                </a>

                {isMenuOpen &&
                    <>
                        <nav className={styles.navMenu}>
                            <a className={styles.menuItem}>Каталог</a>
                            <a className={styles.menuItem}>На заказ</a>
                            <a className={styles.menuItem}>Скидки</a>
                            <a className={styles.menuItem}>Доставка</a>
                            <a className={styles.menuItem}>О нас</a>
                        </nav>

                        <nav className={styles.buttonsMenu}>
                            <button className={styles.menuIcon}><img src={cartIcon} alt="cart"/></button>
                            <button className={styles.menuIcon}><img src={accountIcon} alt="account"/></button>
                        </nav>
                    </>
                }
            </header>

            <div className={styles.bottomLine}></div>
        </div>
    );
}