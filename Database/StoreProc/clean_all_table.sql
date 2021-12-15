CREATE DEFINER=`admin`@`%` PROCEDURE `sp_clean_all_table`(

)
BEGIN
    SET FOREIGN_KEY_CHECKS = 0;
	DELETE FROM asset_category;
    DELETE FROM asset_department;
    DELETE FROM asset_discount;
    DELETE FROM asset_location;
    DELETE FROM asset_product;
    DELETE FROM asset_section;
    DELETE FROM asset_tax;
    DELETE FROM asset_user;
    DELETE FROM asset_vendor;
    DELETE FROM ref_location_product;
    ALTER TABLE ref_location_product auto_increment = 1;
    DELETE FROM ref_location_product_category;
    ALTER TABLE ref_location_product_category auto_increment = 1;
    DELETE FROM ref_location_product_department;
    ALTER TABLE ref_location_product_department auto_increment = 1;
    DELETE FROM ref_location_product_itemcode;
    ALTER TABLE ref_location_product_itemcode auto_increment = 1;
    DELETE FROM ref_location_product_section;
    ALTER TABLE ref_location_product_section auto_increment = 1;
    DELETE FROM ref_location_product_upc;
    ALTER TABLE ref_location_product_upc auto_increment = 1;
    DELETE FROM ref_location_product_vendor;
    ALTER TABLE ref_location_product_vendor auto_increment = 1;
    DELETE FROM ref_location_user;
    ALTER TABLE ref_location_user auto_increment = 1;
    DELETE FROM trans_checkout;
    ALTER TABLE trans_checkout auto_increment = 1;
    DELETE FROM trans_checkout_detail;
    ALTER TABLE trans_checkout_detail auto_increment = 1;
	SET FOREIGN_KEY_CHECKS = 1;
END