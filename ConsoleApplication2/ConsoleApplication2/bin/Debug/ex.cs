 [WebMethod] 
  public string Read_xxt_usersinfo_Data(string name,int PageNum = 0, int Count = 10)   
 { 
 string where = "name LIKE '%"+name+"%'";
 return ReadMyData("xxt_usersinfo",  PageNum,  Count ,where ); 
}
